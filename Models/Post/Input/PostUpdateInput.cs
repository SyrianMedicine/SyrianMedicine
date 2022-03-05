using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Models.Post.Input
{
    public class PostUpdateInput
    {
        public int Id { get; set; }
        [Required]
        public string PostText { get; set; }
        public DAL.Entities.Post.PostType Type { get; set; }
        public List<int> TagsID { get; set; }
    }
    public class PostUpdateInputValidator : AbstractValidator<PostUpdateInput>
    {
        public PostUpdateInputValidator()
        {
            RuleFor(x => x.PostText)
                .NotEmpty().WithMessage("Please enter PostText");
        }
    }
}