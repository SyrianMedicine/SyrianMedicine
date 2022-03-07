using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class PostComment : Comment
    {
        public int PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }
        public override CommentType getCommentType() =>
            CommentType.PostComment;

        public override string getRelatedObjectid() => this.PostId.ToString();
    }
}