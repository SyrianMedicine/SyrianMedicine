using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class SubComment
    {
        public int Id { get; set; }
        [Required]
        public string CommentText { get; set; }
        public DateTime Date { get; set; }
        public bool IsEdited { get; set; }
        [Required]
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public int CommentId { get; set; }
        
        [ForeignKey(nameof(CommentId))]
        public virtual Comment Comment { get; set; }
        public virtual List<SubCommentLike> LikedByList { get; set; }
    }
}