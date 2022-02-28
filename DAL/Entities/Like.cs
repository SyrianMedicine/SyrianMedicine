using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public DateTime LikeDate { get; set; }
        public string UserId { get; set; } 
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
    }
}