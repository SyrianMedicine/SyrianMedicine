using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Entities;
using Models.Follow.Output;

namespace Services.Profiles
{
    public class FollowProfile:Profile
    {
        public FollowProfile()
        {
            CreateMap<Follow, FollowOutput>()
                .ForMember(e => e.UserName, opt => opt.MapFrom(src =>src.User!=null?src.User.UserName:null))
                .ForMember(e => e.FollowedUserName, opt => opt.MapFrom(src => src.FollowedUser!=null?src.FollowedUser.UserName:null));
        }
    }
}