using FluentValidation;
using GradProjectServer.DTO.Courses;

namespace GradProjectServer.Validators.Courses
{
    public class CourseSearchFilterDtoValidator : AbstractValidator<CourseSearchFilterDto>
    {
        public CourseSearchFilterDtoValidator()
        {
            RuleFor(d => d.Count)
                .GreaterThan(0);
            RuleFor(d => d.Offset)
                .GreaterThanOrEqualTo(0);
            RuleFor(d => d.MinCreditHours)
                .GreaterThanOrEqualTo(0)
                .When(d => d.MinCreditHours.HasValue)
                .LessThanOrEqualTo(d => d.MaxCreditHours)
                .When(d => d.MaxCreditHours.HasValue);
            RuleFor(d => d.MaxCreditHours)
                .GreaterThanOrEqualTo(0)
                .When(d => d.MaxCreditHours.HasValue);
        }
    }
}