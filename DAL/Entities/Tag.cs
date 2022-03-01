using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        [Required]
        public String Tagname { get; set; }
        public virtual List<UserTag> Users { get; set; }
        public virtual List<PostTag> Posts { get; set; }
    }
}