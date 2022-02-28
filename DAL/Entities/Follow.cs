using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Follow
    {
        public int Id { get; set; }
        public DateTime FollowDate { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public string FollowedUserId { get; set; }
        [ForeignKey(nameof(FollowedUserId))]
        public virtual User FollowedUser { get; set; }
    }
}