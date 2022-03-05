using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using FluentValidation;

namespace Models.Post.Input
{
    public class PostCreateInput
    {
        [Required]
        public string PostText { get; set; }
        public DAL.Entities.Post.PostType Type { get; set; }
        public List<int> TagsID { get; set; }

    }
    public class PostCreateInputValidator : AbstractValidator<PostCreateInput>
    {
        public PostCreateInputValidator()
        {
            RuleFor(x => x.PostText)
                .NotEmpty().WithMessage("Please enter PostText");
        }
    }
}