using AutoMapper;
using DAL.Entities;
using DAL.Entities.Identity;
using Models.Bed.Inputs;
using Models.Bed.Outputs;
using Models.Department.Inputs;
using Models.Department.Outputs;
using Models.Hospital;
using Models.Hospital.Inputs;
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

            CreateMap<UpdateHospital, Hospital>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.HospitalId));
            CreateMap<UpdateHospital, User>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumer));



            CreateMap<User, RegisterHospitalOutput>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Hospital.Id));

            CreateMap<RegisterHospital, User>();
            CreateMap<RegisterHospital, Hospital>();

            CreateMap<DocumentsHospitalModel, DocumentsHospital>().ReverseMap();

            CreateMap<UpdateDepartment, Department>();
            CreateMap<CreateDepartment, Department>().ReverseMap();
            CreateMap<Department, DepartmentOutput>()
                .ForMember(dest => dest.HospitalName, opt => opt.MapFrom(src => src.Hospital.Name));

            CreateMap<CreateBed, Bed>().ReverseMap();
            CreateMap<Bed, BedOutput>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department.Name));
            CreateMap<UpdateBed, Bed>();
        }
    }
}