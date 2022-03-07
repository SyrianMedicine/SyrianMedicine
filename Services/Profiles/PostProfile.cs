using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Entities;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Models.Post.Input;
using Models.Post.Output;
using Models.Tag.Output;

namespace Services.Profiles
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<Post, PostOutput>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PostText, opt => opt.MapFrom(src => src.PostText))
            .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => src.IsEdited))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(S =>S.Tags!=null?S.Tags.Select(s=>s.Tag).ToList():null ))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(S => S.Type))
            .ForMember(dest => dest.user, opt => opt.MapFrom(S => S.User));
            CreateMap<PostCreateInput, Post>()
           .ForMember(dest => dest.PostText, opt => opt.MapFrom(src => src.PostText))
           .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type)) 
           .ForMember(dest => dest.Date, opt => opt.MapFrom(i => DateTime.Now));
            CreateMap<PostUpdateInput, Post>() 
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.PostText, opt => opt.MapFrom(src => src.PostText))
           .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
           .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(i => true));
        }
    }
}