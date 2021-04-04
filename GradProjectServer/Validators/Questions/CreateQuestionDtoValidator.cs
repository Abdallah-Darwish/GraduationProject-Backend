using FluentValidation;
using GradProjectServer.DTO.Questions;
using GradProjectServer.Services.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.Questions
{
    public class CreateQuestionDtoValidator : AbstractValidator<CreateQuestionDto>
    {
        public CreateQuestionDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.Content)
                .MinimumLength(1)
                .WithMessage($"{nameof(CreateQuestionDto.Content)} can't be null or empty.");
            RuleFor(d => d.Title)
                .MinimumLength(1)
                .WithMessage($"{nameof(CreateQuestionDto.Title)} can't be null or empty.");
            RuleFor(d => d.CourseId)
                .MustAsync(async (id, _) => (await dbContext.Courses.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage(d => $"Course(Id: {d.CourseId}) doesn't exist.");
            RuleFor(d => d.SubQuestions)
                .NotNull()
                .WithMessage($"{nameof(CreateQuestionDto.SubQuestions)} can't be null.")
                .Must(questions => questions.Length >= 1)
                .WithMessage($"{nameof(CreateQuestionDto.SubQuestions)} size must be >= 1.")
                //check if its working or we should use SetValidator
                .InjectValidator();
        }
    }
}
