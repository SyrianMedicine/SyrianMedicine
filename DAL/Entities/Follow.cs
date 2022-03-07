using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required]
        public string UserId { get; set; } 
        public virtual User User { get; set; }
        [Required]
        public string FollowedUserId { get; set; } 
        public virtual User FollowedUser { get; set; }
    }
}