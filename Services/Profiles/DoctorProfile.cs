using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using Models.Doctor.Inputs;
using Models.Doctor.Outputs;
using Models.Helper;

namespace Services.Profiles
{
    public class DoctorProfile : Profile
    {
        public DoctorProfile()
        {
            CreateMap<Doctor, DoctorOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => "https://syrian-medicine.herokuapp.com/" + src.User.PictureUrl))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.User.Location))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.User.State))
                .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.User.HomeNumber))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));

            CreateMap<Doctor, MostDoctorsRated>()
                .ForMember(dest => dest.AboutMe, opt => opt.MapFrom(src => src.AboutMe))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.User.PictureUrl))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City));

            CreateMap<PagedList<Doctor>, PagedList<DoctorOutput>>();
            CreateMap<PagedList<Doctor>, PagedList<MostDoctorsRated>>();

            CreateMap<RegisterDoctor, User>();
            CreateMap<RegisterDoctor, Doctor>();
            CreateMap<UpdateDoctor, User>();
            CreateMap<UpdateDoctor, Doctor>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DoctorId));

            CreateMap<User, LoginOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));
            CreateMap<User, RegisterDoctorOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Doctor.Id));

            CreateMap<DocumentsDoctorModel, DocumentsDoctor>().ReverseMap();

            CreateMap<ReserveDoctor, ReserveDoctorOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Doctor.User.FirstName + " " + src.Doctor.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Doctor.User.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Doctor.User.UserName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Doctor.User.PhoneNumber))
                .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.Doctor.User.HomeNumber));
            CreateMap<CheckReserve, ReserveDoctor>();
        }
    }
}