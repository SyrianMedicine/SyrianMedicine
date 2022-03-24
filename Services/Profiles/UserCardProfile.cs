using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Entities.Identity;
using Models.UserCard;

namespace Services.Profiles
{
    public class UserCardProfile : Profile
    {
        public UserCardProfile()
        {
            CreateMap<User, UserCardOutput>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Hospital != null ? src.Hospital.Name : src.FirstName + " " + src.LastName))
            .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.PictureUrl))
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => src.UserType))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));
        }
    }
}