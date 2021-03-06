using FluentValidation;
using GradProjectServer.DTO.Questions;
using GradProjectServer.Services.EntityFramework;

namespace GradProjectServer.Validators.Questions
{
    public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
    {
        public CreateQuestionDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.Content)
                .NotEmpty();
            RuleFor(d => d.Title)
                .NotEmpty();
            RuleFor(d => d.CourseId)
                .MustAsync(async (id, _) => (await dbContext.Courses.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage("Course({PropertyValue}: {PropertyName}) doesn't exist.");
        }
    }
}