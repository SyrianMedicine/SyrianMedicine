using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Entities.Identity.Enums;
using Models.Account.Outputs;
using Models.Helper;
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

            CreateMap<ReserveDateWithDoctor, ReserveDoctor>()
                .ForMember(dest => dest.ReserveType, opt => opt.MapFrom(src => src.ReserveType));
            CreateMap<UpdateReserveDateWithDoctor, ReserveDoctor>()
                .ForMember(dest => dest.ReserveType, opt => opt.MapFrom(src => src.ReserveType));

            CreateMap<ReserveDateWithNurse, ReserveNurse>()
                .ForMember(dest => dest.ReserveType, opt => opt.MapFrom(src => src.ReserveType));
            CreateMap<UpdateReserveDateWithNurse, ReserveNurse>()
                .ForMember(dest => dest.ReserveType, opt => opt.MapFrom(src => src.ReserveType));

            CreateMap<ReserveBedInHospital, ReserveHospital>();
            CreateMap<UpdateReserveBedInHospital, ReserveHospital>();

            CreateMap<ReserveDoctor, SickReserveOutput>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Doctor!=null&&src.Doctor.User!=null? src.Doctor.User.fullName:""))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ReserveState, opt => opt.MapFrom(src => src.ReserveState.ToString()))
                .ForMember(dest => dest.TimeReverse, opt => opt.MapFrom(src => src.TimeReverse))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title)) 
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Doctor"))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Doctor!=null&&src.Doctor.User!=null? src.Doctor.User.UserName:""));

            CreateMap<ReserveNurse, SickReserveOutput>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Nurse!=null&&src.Nurse.User!=null? src.Nurse.User.fullName:""))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ReserveState, opt => opt.MapFrom(src => src.ReserveState.ToString()))
                .ForMember(dest => dest.TimeReverse, opt => opt.MapFrom(src => src.TimeReverse))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title)) 
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Nurse"))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Nurse!=null&&src.Nurse.User!=null? src.Nurse.User.UserName:""));


                CreateMap<ReserveHospital, SickReserveOutput>()
                .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Bed!=null&&src.Bed.Hospital!=null? src.Bed.Hospital.Name:""))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ReserveState, opt => opt.MapFrom(src => src.ReserveState.ToString()))
                .ForMember(dest => dest.TimeReverse, opt => opt.MapFrom(src => src.DateTime))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title)) 
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => "Hospital"))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Bed!=null&&src.Bed.Hospital!=null&&src.Bed.Hospital.User!=null?  src.Bed.Hospital.User.UserName:""));
                CreateMap<PagedList<ReserveDoctor>, PagedList<SickReserveOutput>>();
                CreateMap<PagedList<ReserveNurse>, PagedList<SickReserveOutput>>();
                CreateMap<PagedList<ReserveHospital>, PagedList<SickReserveOutput>>();
        }
    }
}