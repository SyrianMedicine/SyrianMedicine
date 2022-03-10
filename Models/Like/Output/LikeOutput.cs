using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using Models.Comment.Output;
using Models.Post.Output;
using Models.UserCard;

namespace Models.Like.Output
{
    public class LikeOutput
    {
        public int Id { get; set; }
        public UserCardOutput user{ get; set; }
        public string LikeType { get; set; }
        public string objectid { get; set; } 
    }
}