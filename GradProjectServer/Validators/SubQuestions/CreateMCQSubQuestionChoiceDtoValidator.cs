using FluentValidation;
using GradProjectServer.DTO.SubQuestions;

namespace GradProjectServer.Validators.SubQuestions
{
    public class CreateMCQSubQuestionChoiceDtoValidator : AbstractValidator<CreateMCQSubQuestionChoiceDto>
    {
        public CreateMCQSubQuestionChoiceDtoValidator()
        {
            RuleFor(d => d.Content)
                .NotEmpty();
            RuleFor(d => d.Weight)
                .InclusiveBetween(-1, 1)
                .WithMessage("{PropertyName} must be in range [-1, 1].");
        }
    }
}
