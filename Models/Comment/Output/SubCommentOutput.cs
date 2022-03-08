using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.UserCard;

namespace Models.Comment.Output
{
    public class SubCommentOutput
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Datetime { get; set; }
        public bool IsEdited { get; set; }
        public UserCardOutput User { get; set; }
        public int Commentid { get; set; }
        public CommentOutput OnComment { get; set; }
    }
}