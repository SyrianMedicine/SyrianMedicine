using AutoMapper;
using DAL.Entities.Identity;
using Models.Sick.Inputs;
using Models.Sick.Outputs;

namespace Services.Profiles
{
    public class SickProfile : Profile
    {
        public SickProfile()
        {
            CreateMap<User, SickOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + src.LastName));
            CreateMap<User, LoginSickOutput>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + src.LastName));
            CreateMap<RegisterSick, User>();
            CreateMap<User, RegisterSickOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + src.LastName));
            CreateMap<UpdateSick, User>();
        }
    }
}