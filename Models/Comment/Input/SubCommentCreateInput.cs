using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Models.Comment.Input
{
    public class SubCommentCreateInput
    {
        [Required]
        public string CommentText { get; set; }
        public int CommentId { get; set; }
    }
    public class SubCommentCreateInputValidator : AbstractValidator<SubCommentCreateInput>
    {
        public SubCommentCreateInputValidator()
        {
            RuleFor(x => x.CommentText)
                .NotEmpty().WithMessage("Please enter PostText");
        }
    }
}