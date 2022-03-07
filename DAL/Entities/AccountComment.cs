using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class AccountComment : Comment
    {
        public string OnAccountId { get; set; }
        [ForeignKey(nameof(OnAccountId))]
        public virtual User OnAccount { get; set; }
        public override CommentType getCommentType()
        {
            return CommentType.AccountComment;
        }
        public override string getRelatedObjectid() => OnAccount != null ? this.OnAccount.UserName : null;
    }
}