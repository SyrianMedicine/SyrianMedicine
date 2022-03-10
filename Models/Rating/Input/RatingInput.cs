using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Models.Rating.Input
{
    public class RatingInput
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public DAL.Entities.Rating.Rate StarsNumber { get; set; }
    }
    public class RatingInputValidator : AbstractValidator<RatingInput>
    {
        public RatingInputValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Please enter Username");
        }
    }
}
