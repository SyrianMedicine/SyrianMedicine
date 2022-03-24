using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class CommentLike:Like
    { 
        public int CommentID { get; set; }
        
        [ForeignKey(nameof(CommentID))]
        public virtual Comment Comment { get; set; } 
        public override LikeType GetLikeType()
        {
            return LikeType.CommentLike;
        }
        public override string GetObjectId()
        {
            return CommentID.ToString();
        }
    }
}