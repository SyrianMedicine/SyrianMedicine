using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Models.Comment.Input
{
    public class CommentUpdateInput
    {
        public int Id { get; set; }
        [Required]
        public string CommentText { get; set; }
    }
    public class CommentUpdateInputValidator : AbstractValidator<CommentUpdateInput>
    {
        public CommentUpdateInputValidator()
        {
            RuleFor(x => x.CommentText)
                .NotEmpty().WithMessage("Please enter CommentText");
        }
    }
}