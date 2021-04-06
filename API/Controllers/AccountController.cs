using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {

        private readonly DataContext _context;
          private readonly  ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService=tokenService;

        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerdto)
        {
            if (await UserExists(registerdto.UserName))
            {
                return BadRequest("Username already exists");
            }

            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerdto.UserName.ToLower(),
                PaswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password)),
                PaswordSalt = hmac.Key
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDto{
                Username=user.UserName,
                Token=_tokenService.CreateToken(user)
            };
        }

        
        
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user=await _context.Users.
            Include(p=>p.Photos).
            SingleOrDefaultAsync(x=>x.UserName==loginDto.UserName);
            if(user==null)
            {
                return Unauthorized("Invalid UserName");
            }
             
             using var hmac=new HMACSHA512(user.PaswordSalt);

             var computeHash=hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

             for(int i=0;i<computeHash.Length;i++)
             {
                 if(computeHash[i]!=user.PaswordHash[i])
                 {
                     return Unauthorized("Invalid Password");
                 }
             }

             return   new UserDto{
                Username=user.UserName,
                Token=_tokenService.CreateToken(user),
                PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain)?.Url
            };
        }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
}