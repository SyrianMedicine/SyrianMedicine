using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class SubCommentLike : Like
    {
        public int SubCommentID { get; set; }
        public virtual SubComment SubComment { get; set; }
    }
}