using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class SubComment : Comment
    {
        public int CommentId { get; set; }

        [ForeignKey(nameof(CommentId))]
        public virtual Comment Comment { get; set; }
        public override CommentType getCommentType()
        {
            return CommentType.SubComment;
        }
        public override string getRelatedObjectid()
        {
            return CommentId+"";
        }
    }
}