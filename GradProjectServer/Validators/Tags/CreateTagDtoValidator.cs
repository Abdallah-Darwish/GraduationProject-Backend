using FluentValidation;
using GradProjectServer.DTO.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.Tags
{
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty();
        }
    }
}
