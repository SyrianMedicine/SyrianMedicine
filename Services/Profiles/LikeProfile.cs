using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Entities;
using Models.Like.Output;

namespace Services.Profiles
{
    public class LikeProfile : Profile
    {
        public LikeProfile()
        {
            CreateMap<Like, LikeOutput>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LikeType, opt => opt.MapFrom(src => src.GetLikeType().ToString()))
                .ForMember(dest => dest.objectid, opt => opt.MapFrom(src => src.GetObjectId()))
                .ForMember(dest => dest.user, opt => opt.MapFrom(src => src.User));
        }
    }
}