using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DAL.Entities;
using FluentValidation;
using  Microsoft.AspNetCore.Http;
namespace Models.Post.Input
{
    public class PostCreateInput
    {
        [Required]
        public string PostText { get; set; }
        [Required]
        public string PostTitle { get; set; }
        public IFormFile Media { get; set; }
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