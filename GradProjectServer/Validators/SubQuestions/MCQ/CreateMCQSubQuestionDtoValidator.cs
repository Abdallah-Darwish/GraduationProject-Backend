using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GradProjectServer.Validators.SubQuestions.MCQ
{
    public class CreateMCQSubQuestionDtoValidator : AbstractValidator<CreateMCQSubQuestionDto>
    {
        public CreateMCQSubQuestionDtoValidator(IServiceProvider sp)
        {
            Include(sp.GetRequiredService<CreateSubQuestionDtoValidator>());
            RuleForEach(d => d.Choices)
                .SetValidator(sp.GetRequiredService<CreateMCQSubQuestionChoiceDtoValidator>());
        }
    }
}