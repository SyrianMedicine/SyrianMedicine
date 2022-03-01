using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities.Identity;

namespace DAL.Entities
{
    public class Post
    {
        /// <summary>
        ///post=1 منشور عادي 
        /// <br/>
        ///Question=2 استفسار
        /// </summary>
        public enum PostType{post=1,Question}
        public int Id { get; set; }
        [Required]
        public string PostText { get; set; }
        public PostType Type{get;set;}
        public DateTime Date { get; set; }
        public bool IsEdited { get; set; }
        [Required]
        public string UserId { get; set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }
        public virtual List<PostTag> Tags { get; set; }
        public virtual List<PostComment> Comments { get; set; }
        public virtual List<PostLike> LikedByList { get; set; }
    }
}