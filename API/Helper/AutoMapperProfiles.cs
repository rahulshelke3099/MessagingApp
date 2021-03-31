using System.Linq;
using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helper
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser,MemberDto>()
            .ForMember(destinationMember=>destinationMember.PhotoUrl,
            IMappingOperationOptions=>IMappingOperationOptions
            .MapFrom(sourceMember=>
            sourceMember.Photos.FirstOrDefault(x=>x.IsMain).Url))

            .ForMember(destinationMember=>destinationMember.Age,
             IMappingOperationOptions=>IMappingOperationOptions
            .MapFrom(sourceMember=>
            sourceMember.DateOfBirth.CalculateAge()));



            CreateMap<Photo,PhotoDto>();
        }
        
    }
}