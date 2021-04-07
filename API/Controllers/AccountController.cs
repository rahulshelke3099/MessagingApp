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
using AutoMapper;
namespace API.Controllers
{
    public class AccountController : BaseApiController
    {

        private readonly DataContext _context;
          private readonly  ITokenService _tokenService;
           private readonly  IMapper _mapper;
        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService=tokenService;
            _mapper=mapper;

        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerdto)
        {
            if (await UserExists(registerdto.UserName))
            {
                return BadRequest("Username already exists");
            }
             var user=_mapper.Map<AppUser>(registerdto);
            using var hmac = new HMACSHA512();

          
                user.UserName = registerdto.UserName.ToLower();
                user.PaswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password));
                user.PaswordSalt = hmac.Key;
           

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDto{
                Username=user.UserName,
                Token=_tokenService.CreateToken(user),
                KnownAs=user.KnownAs,
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
                PhotoUrl=user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
                 KnownAs=user.KnownAs,
            };
        }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
}