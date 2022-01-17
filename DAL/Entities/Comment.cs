using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public DateTime Date { get; set; }
        public bool IsEdited { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public virtual List<CommentLike> LikedByList { get; set; }
        public virtual List<SubComment> SubComments { get; set; }

    }
}