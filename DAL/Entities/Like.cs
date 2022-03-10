using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Like
    {
        public enum LikeType { BaseLike = 1, CommentLike, PostLike, SubCommentLike }
        public int Id { get; set; }
        public DateTime LikeDate { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        public virtual LikeType GetLikeType()
        {
            return LikeType.BaseLike;
        }
        public virtual string GetObjectId()
        {
            return null;
        }
    }
}