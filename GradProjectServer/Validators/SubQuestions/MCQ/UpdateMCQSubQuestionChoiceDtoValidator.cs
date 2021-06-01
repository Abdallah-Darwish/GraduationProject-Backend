using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.EntityFramework;

namespace GradProjectServer.Validators.SubQuestions
{
    public class UpdateMCQSubQuestionChoiceDtoValidator : AbstractValidator<UpdateMCQSubQuestionChoiceDto>
    {
        public UpdateMCQSubQuestionChoiceDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.Id)
                .MustAsync(async (id, _) =>
                    (await dbContext.MCQSubQuestionsChoices.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage("MCQSubQuestionChoice(Id: {PropertyValue}) doesn't exist");
            RuleFor(d => d.Content)
                .NotEmpty()
                .When(d => d.Content != null);
            RuleFor(d => d.Weight)
                .InclusiveBetween(-1.0f, 1.0f)
                .When(d => d.Weight.HasValue);
        }
    }
}