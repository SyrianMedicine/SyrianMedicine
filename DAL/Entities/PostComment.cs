using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class PostComment : Comment
    { 
        public int PostId { get; set; }
        public virtual Post Post { get; set; }
    }
}