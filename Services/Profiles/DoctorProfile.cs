using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using Models.Doctor.Outputs;

namespace Services.Profiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<Doctor, DoctorsOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.User.PictureUrl))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.User.Location))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.User.State))
                .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.User.HomeNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender));

            CreateMap<DocumentsDoctorModel, DocumentsDoctor>().ReverseMap();
        }
    }
}