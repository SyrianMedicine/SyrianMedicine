using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.Post.Output;
using Models.UserCard;

namespace Models.Comment.Output
{
    public class CommentOutput
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Datetime { get; set; }
        public bool IsEdited { get; set; }
        public string RelatedObjectType { get; set; }
        public string RealtedObjectId { get; set; }
        public UserCardOutput User { get; set; }
        public PostOutput OnPost { get; set; }
        public UserCardOutput OnAccount { get; set; }  
        public CommentOutput OnComment { get; set; }
    }
}