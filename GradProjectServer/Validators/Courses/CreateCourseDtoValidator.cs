using FluentValidation;
using GradProjectServer.DTO.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.Courses
{
    public class CreateCourseDtoValidator : AbstractValidator<CreateCourseDto>
    {
        public CreateCourseDtoValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty();
            RuleFor(d => d.CreditHours)
                .GreaterThanOrEqualTo(1)
                .WithMessage("{PropertyName} must be >= {ComparisonValue}.");
        }
    }
}
