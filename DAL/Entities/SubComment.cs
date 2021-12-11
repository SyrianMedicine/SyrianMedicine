using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class SubComment
    {
        public int Id { get; set; }
        public String CommentText { get; set; }
        public DateTime Date { get; set; }
        public bool IsEdited { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int CommentId { get; set; }
        public virtual Comment Comment { get; set; }
        public virtual List<SubCommentLike> LikedByList { get; set; }
    }
}