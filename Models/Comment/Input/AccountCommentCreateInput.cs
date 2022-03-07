using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Models.Comment.Input
{
    public class AccountCommentCreateInput
    { 
        [Required]
        public string CommentText { get; set; }
        [Required]
        public string AccountUserName { get; set; }
    }
    public class AccountCommentCreateInputValidator : AbstractValidator<AccountCommentCreateInput>
    {
        public AccountCommentCreateInputValidator()
        {
            RuleFor(x => x.CommentText)
                .NotEmpty().WithMessage("Please enter CommentText");
            RuleFor(x => x.AccountUserName)
                .NotEmpty().WithMessage("Please enter AccountId");
        }
    }
}