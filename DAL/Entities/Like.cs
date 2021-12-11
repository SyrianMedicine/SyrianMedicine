using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public DateTime LikeDate { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
    }
}