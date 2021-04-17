using FluentValidation;
using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.Services.EntityFramework;

namespace GradProjectServer.Validators.Exams
{
    public class CreateExamSubQuestionDtoValidator : AbstractValidator<CreateExamSubQuestionDto>
    {
        public CreateExamSubQuestionDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.Weight)
                .GreaterThan(0.0f);
            RuleFor(d => d.SubQuestionId)
                .MustAsync(async (id, _) =>
                {
                    var subQuestion = await dbContext.SubQuestions.FindAsync(id).ConfigureAwait(false);
                    return subQuestion != null && subQuestion.Question.IsApproved;
                })
                .WithMessage("SubQuestion(Id: {PropertyValue}) doesn't exist or isn't approved yet.");
        }
    }
}
