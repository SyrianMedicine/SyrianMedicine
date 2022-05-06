using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using Models.Account.Outputs;
using Models.Admin.Inputs;
using Models.Admin.Outputs;
using Models.Common;
using Models.Helper;

namespace Services.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<User, LoginOutput>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType));
                
            CreateMap<UpdateAdmin, User>();

            CreateMap<PagedList<City>, PagedList<OptionDto>>();

            CreateMap<Doctor, ValidateAccountOutput>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.User.Date))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.DocumentsDoctor.Select(e => e.UrlFile)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));

            CreateMap<Nurse, ValidateAccountOutput>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.User.Date))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.DocumentsNurse.Select(e => e.UrlFile)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));

            CreateMap<Hospital, ValidateAccountOutput>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.User.Date))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.DocumentsHospital.Select(e => e.UrlFile)))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName));
            CreateMap<PagedList<Nurse>, PagedList<ValidateAccountOutput>>();
            CreateMap<PagedList<Doctor>, PagedList<ValidateAccountOutput>>();
            CreateMap<PagedList<Hospital>, PagedList<ValidateAccountOutput>>();
        }
    }
}