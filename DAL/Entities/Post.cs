using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string PostText { get; set; }
        public DateTime Date { get; set; }
        public bool IsEdited { get; set; }
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public virtual List<PostTag> Tags { get; set; }
        public virtual List<PostComment> Comments { get; set; }
        public virtual List<PostLike> LikedByList { get; set; }
    }
}