using FluentValidation;
using GradProjectServer.DTO.Courses;

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
