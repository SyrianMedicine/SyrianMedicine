using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Models.Tag.Output;
using Models.UserCard; 

namespace Models.Post.Output
{
    public class PostOutput
    {

        public int Id { get; set; }
        [Required]
        public string PostText { get; set; }
        public DAL.Entities.Post.PostType Type { get; set; }
        public bool IsEdited { get; set; }
        public UserCardOutput user { get; set; }
        public List<TagOutput> Tags { get; set; }

    }
}