using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public String PostText { get; set; }
        public DateTime Date { get; set; }
        public bool IsEdited { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual List<PostTag> Tags { get; set; }
        public virtual List<PostComment> Comments { get; set; }
        public virtual List<PostLike> LikedByList { get; set; }
    }
}