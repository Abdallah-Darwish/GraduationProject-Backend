using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.SubQuestions
{
    public class CreateMCQSubQuestionChoiceDtoValidator : AbstractValidator<CreateMCQSubQuestionChoiceDto>
    {
        public CreateMCQSubQuestionChoiceDtoValidator()
        {
            RuleFor(d => d.Content)
                .MinimumLength(1)
                .WithMessage($"{nameof(CreateMCQSubQuestionChoiceDto.Content)} can't be null or empty");
            RuleFor(d => d.Weight)
                .InclusiveBetween(-1, 1)
                .WithMessage($"{nameof(CreateMCQSubQuestionChoiceDto.Weight)} must be in range [-1, 1].");
        }
    }
}
