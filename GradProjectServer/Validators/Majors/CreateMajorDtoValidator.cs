using FluentValidation;
using GradProjectServer.DTO.Majors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.Majors
{
    public class CreateMajorDtoValidator : AbstractValidator<CreateMajorDto>
    {
        public CreateMajorDtoValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty();
        }
    }
}
