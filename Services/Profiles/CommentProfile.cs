using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Entities;
using Models.Comment.Input;
using Models.Comment.Output;
using Models.Helper;

namespace Services.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentOutput>()
                .ForMember(dest => dest.Datetime, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => src.IsEdited))
                .ForMember(dest => dest.RelatedObjectType, opt => opt.MapFrom(src => src.getCommentType().ToString()))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.CommentText))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.OnPost, opt => opt.MapFrom(src => src is PostComment ? (src as PostComment).Post : null)) 
                .ForMember(dest => dest.OnComment, opt => opt.MapFrom(src => src is SubComment ? (src as SubComment).Comment : null))
                .ForMember(dest => dest.OnAccount, opt => opt.MapFrom(src => src is AccountComment ? (src as AccountComment).OnAccount : null))
                .ForMember(dest => dest.RealtedObjectId, opt => opt.MapFrom(src => src.getRelatedObjectid()));
            CreateMap<AccountCommentCreateInput, AccountComment>()
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.CommentText))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<PostCommentCreateInput, PostComment>()
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.CommentText))
                .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.Postid))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<SubCommentCreateInput, SubComment>()
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.CommentText))
                .ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.CommentId))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateTime.Now));  
            CreateMap<CommentUpdateInput, Comment>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsEdited, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CommentText, opt => opt.MapFrom(src => src.CommentText)); 
            CreateMap<PagedList<Comment>, PagedList<CommentOutput>>();

        }
    }
}