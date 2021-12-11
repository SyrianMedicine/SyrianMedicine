using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Follow
    {
        public int Id { get; set; }
        public DateTime FollowDate { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int FollowedUserId { get; set; }
        public virtual User FollowedUser { get; set; }
    }
}