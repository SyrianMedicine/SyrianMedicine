using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Entities;
using Models.Comment.Output;

namespace Services.Profiles
{
    public class PostCommentProfile : Profile
    {
        public PostCommentProfile()
        {
            CreateMap<Comment, CommentOutput>()
                .ForMember(dest => dest.Datetime, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => src.IsEdited))
                .ForMember(dest => dest.RelatedObjectType, opt => opt.MapFrom(src => src.getCommentType().ToString()))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.CommentText))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.RealtedObjectId, opt => opt.MapFrom(src => src.getRelatedObjectid()));
        }
    }
}