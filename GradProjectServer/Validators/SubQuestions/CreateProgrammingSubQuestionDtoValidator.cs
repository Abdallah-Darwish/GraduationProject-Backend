using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GradProjectServer.Validators.SubQuestions
{
    public class CreateProgrammingSubQuestionDtoValidator : AbstractValidator<CreateProgrammingSubQuestionDto>
    {
        public CreateProgrammingSubQuestionDtoValidator(IServiceProvider sp)
        {
            Include(sp.GetRequiredService<CreateSubQuestionDtoValidator>());
            RuleFor(d => d.Checker)
                .InjectValidator();
        }
    }
}
