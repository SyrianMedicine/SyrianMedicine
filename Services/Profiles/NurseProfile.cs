using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using Models.Nurse;
using Models.Nurse.Outputs;

namespace Services.Profiles
{
    public class NurseProfile : Profile
    {
        public NurseProfile()
        {
            CreateMap<Nurse, NurseOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.User.PictureUrl))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.User.Location))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.User.State))
                .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.User.HomeNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));


            CreateMap<DocumentsNurseModel, DocumentsNurse>().ReverseMap();
        }
    }
}