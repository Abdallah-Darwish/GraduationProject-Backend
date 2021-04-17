using FluentValidation;
using GradProjectServer.DTO.Courses;
using GradProjectServer.Services.EntityFramework;

namespace GradProjectServer.Validators.Courses
{
    public class UpdateCourseDtoValidator : AbstractValidator<UpdateCourseDto>
    {
        public UpdateCourseDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.Id)
                .MustAsync(async (id, _) => (await dbContext.Courses.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage("Course(Id: {PropertyValue}) doesn't exist.");
            RuleFor(d => d.Name)
               .NotEmpty()
               .When(d => d.Name != null);
            RuleFor(d => d.CreditHours)
                .GreaterThanOrEqualTo(1)
                .WithMessage("{PropertyName} must be >= {ComparisonValue}.")
                .When(d => d.CreditHours.HasValue);
        }
    }
}
