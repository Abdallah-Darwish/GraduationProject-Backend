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
                .InclusiveBetween(-1.0f, 1.0f);
        }
    }
}