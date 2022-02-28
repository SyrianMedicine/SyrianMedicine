using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Entities;
using Models.Tag.Input;
using Models.Tag.Output;

namespace Services.Profiles
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagOutput>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Tagname));
            CreateMap<TagUpdateInput, Tag>()
           .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Tagname, opt => opt.MapFrom(src => src.Name)); 
            CreateMap<TagCreateInput, Tag>() 
           .ForMember(dest => dest.Tagname, opt => opt.MapFrom(src => src.Name));
        }
    }
}