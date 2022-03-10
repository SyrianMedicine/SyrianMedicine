using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class SubCommentLike : Like
    {
        public int SubCommentID { get; set; }

        [ForeignKey(nameof(SubCommentID))]
        public virtual SubComment SubComment { get; set; }
        public override LikeType GetLikeType()
        {
            return LikeType.SubCommentLike;
        }
        public override string GetObjectId()
        {
            return SubCommentID.ToString();
        }
    }
}