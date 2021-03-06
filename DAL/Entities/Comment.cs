using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Comment
    {
        public enum CommentType { UnKnownType = 1, PostComment, AccountComment ,SubComment}
        public int Id { get; set; }
        [Required]
        public string CommentText { get; set; }
        public DateTime Date { get; set; }
        public bool IsEdited { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public virtual List<CommentLike> LikedByList { get; set; }
        public virtual List<SubComment> SubComments { get; set; }
        public virtual CommentType getCommentType()
        {
            return CommentType.UnKnownType;
        }
        public virtual string getRelatedObjectid()
        {
            return null;
        }
    }
}