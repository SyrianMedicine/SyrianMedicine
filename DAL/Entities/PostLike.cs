using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class PostLike : Like
    {
        public int PostID { get; set; }
        
        [ForeignKey(nameof(PostID))]
        public virtual Post Post { get; set; }

    }
}