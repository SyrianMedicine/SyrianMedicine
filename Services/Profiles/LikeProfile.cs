using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Entities;
using Models.Helper;
using Models.Like.Output;

namespace Services.Profiles
{
    public class LikeProfile : Profile
    {
        public LikeProfile()
        {
            CreateMap<PagedList<Like>, PagedList<LikeOutput>>();
            CreateMap<Like, LikeOutput>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LikeType, opt => opt.MapFrom(src => src.GetLikeType().ToString()))
                .ForMember(dest => dest.objectid, opt => opt.MapFrom(src => src.GetObjectId())) 
                .ForMember(dest => dest.OnComment, opt => opt.MapFrom(src => src is CommentLike?(src as CommentLike).Comment:null )) 
                .ForMember(dest => dest.Onpost, opt => opt.MapFrom(src => src is PostLike?(src as PostLike).Post:null))
                .ForMember(dest => dest.user, opt => opt.MapFrom(src => src.User));
        }
    }
}