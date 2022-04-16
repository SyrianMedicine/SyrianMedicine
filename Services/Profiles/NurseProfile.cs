using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using Models.Doctor.Outputs;
using Models.Helper;
using Models.Nurse;
using Models.Nurse.Inputs;
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
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.User.PictureUrl == null ? null : "https://syrian-medicine.herokuapp.com" + src.User.PictureUrl))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.User.Location))
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.User.State))
                .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.User.HomeNumber))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber));


            CreateMap<Nurse, MostNursesRated>()
                .ForMember(dest => dest.AboutMe, opt => opt.MapFrom(src => src.AboutMe))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Specialization, opt => opt.MapFrom(src => src.Specialization))
                .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.User.PictureUrl))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.User.City));

            CreateMap<PagedList<Nurse>, PagedList<NurseOutput>>();
            CreateMap<PagedList<Nurse>, PagedList<MostNursesRated>>();

            CreateMap<User, LoginNurseOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName));

            CreateMap<LoginNurseInput, User>();
            CreateMap<LoginNurseInput, Nurse>();

            CreateMap<RegisterNurse, User>();
            CreateMap<RegisterNurse, Nurse>();

            CreateMap<UpdateNurse, User>();
            CreateMap<UpdateNurse, Nurse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.NurseId));

            CreateMap<User, RegisterNurseOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.NurseId, opt => opt.MapFrom(src => src.Nurse.Id));


            CreateMap<DocumentsNurseModel, DocumentsNurse>().ReverseMap();

            CreateMap<ReserveNurse, ReserveNurseOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Nurse.User.FirstName + " " + src.Nurse.User.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Nurse.User.Email))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Nurse.User.UserName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Nurse.User.PhoneNumber))
                .ForMember(dest => dest.HomeNumber, opt => opt.MapFrom(src => src.Nurse.User.HomeNumber));
            CreateMap<CheckReserve, ReserveNurse>();
        }
    }
}