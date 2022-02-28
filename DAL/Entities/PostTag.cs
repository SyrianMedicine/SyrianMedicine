using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class PostTag
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        
        [ForeignKey(nameof(TagId))]
        public virtual Tag Tag { get; set; }
        public int PostId { get; set; }
        
        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }

    }
}