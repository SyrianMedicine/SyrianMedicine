using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Models.Tag.Input
{
    /// <summary>
    /// Like (قلبية , عصبية ,)
    /// </summary>
    public class TagCreateInput
    {
        /// <summary>
        /// tag name   
        /// </summary>
        /// <value>Like (قلبية , عصبية ,) </value>
        
        [Required]
        public String Name { get; set; }
    }
    public class TagCreateInputValidator : AbstractValidator<TagCreateInput>
    {
        public TagCreateInputValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Please enter Tag Name");
            RuleFor(x => x.Name)
            .MaximumLength(25).WithMessage(Input => Input.Name + "has more than 25 charcter");

        }
    }
}