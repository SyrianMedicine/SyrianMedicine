using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Models.Tag.Input
{
    public class TagUpdateInput
    {
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// tag name   
        /// </summary>
        /// <value>Like (قلبية , عصبية , دوار,صداع) </value>
        [Required]
        public String Name { get; set; }
    }
    public class TagUpdateInputValidator : AbstractValidator<TagUpdateInput>
    {
        public TagUpdateInputValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("enter Id");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Please enter Tag Name");
            RuleFor(x => x.Name)
            .MaximumLength(25).WithMessage(Input => Input.Name + "has more than 25 charcter");

        }
    }
}