using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Models.Comment.Input
{
    public class PostCommentCreateInput
    { 
        [Required]
        public string CommentText { get; set; }
        public int Postid { get; set; }
    }
        public class PostCommentCreateInputValidator : AbstractValidator<PostCommentCreateInput>
    {
        public PostCommentCreateInputValidator()
        {
            RuleFor(x => x.CommentText)
                .NotEmpty().WithMessage("Please enter PostText");
        }
    }
}