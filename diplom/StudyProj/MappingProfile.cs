using AutoMapper;
using Domain.DTO;
using Domain.Models;

namespace StudyProj
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<UserForRegistrationDto, User>()
                .ForMember(u => u.UserName, opt => opt.MapFrom(x => x.Email));
        }
    }
}
