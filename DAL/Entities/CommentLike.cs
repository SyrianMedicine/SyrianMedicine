using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class CommentLike:Like
    { 
        public int CommentID { get; set; }
        
        [ForeignKey(nameof(CommentID))]
        public virtual Comment Comment { get; set; }
    }
}