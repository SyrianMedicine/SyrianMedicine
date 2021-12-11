using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class PostLike : Like
    {
        public int PostID { get; set; }
        public virtual Post Post { get; set; }

    }
}