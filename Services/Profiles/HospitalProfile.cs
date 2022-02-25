using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using Models.Hospital;
using Models.Hospital.Outputs;

namespace Services.Profiles
{
    public class HospitalProfile : Profile
    {
        public HospitalProfile()
        {
            CreateMap<Hospital, HospitalOutput>()
                .ForMember(e => e.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(e => e.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(e => e.PictureUrl, opt => opt.MapFrom(src => src.User.PictureUrl))
                .ForMember(e => e.Location, opt => opt.MapFrom(src => src.User.Location))
                .ForMember(e => e.PhoneNumer, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(e => e.HomeNumber, opt => opt.MapFrom(src => src.User.HomeNumber))
                .ForMember(e => e.City, opt => opt.MapFrom(src => src.User.City));

            CreateMap<Hospital, RegisterHospitalOutput>()
                .ForMember(e => e.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<DocumentsHospitalModel, DocumentsHospital>().ReverseMap();

        }
    }
}