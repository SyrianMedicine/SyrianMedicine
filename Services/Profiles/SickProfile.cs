using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using Models.Sick;
using Models.Sick.Inputs;
using Models.Sick.Outputs;

namespace Services.Profiles
{
    public class SickProfile : Profile
    {
        public SickProfile()
        {
            CreateMap<User, SickOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + src.LastName))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.PictureUrl == null ? null : "https://syrian-medicine.herokuapp.com" + src.PictureUrl));
            CreateMap<User, LoginSickOutput>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + src.LastName));
            CreateMap<RegisterSick, User>();
            CreateMap<User, RegisterSickOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + src.LastName));
            CreateMap<UpdateSick, User>();

            CreateMap<ReserveDateWithDoctor, ReserveDoctor>();
            CreateMap<UpdateReserveDateWithDoctor, ReserveDoctor>();

            CreateMap<ReserveDateWithNurse, ReserveNurse>();
            CreateMap<UpdateReserveDateWithNurse, ReserveNurse>();

            CreateMap<ReserveBedInHospital, ReserveHospital>();
            CreateMap<UpdateReserveBedInHospital, ReserveHospital>();
        }
    }
}