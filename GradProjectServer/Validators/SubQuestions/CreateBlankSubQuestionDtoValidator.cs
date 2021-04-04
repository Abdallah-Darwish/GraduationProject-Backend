using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.SubQuestions
{
    public class CreateBlankSubQuestionDtoValidator : AbstractValidator<CreateBlankSubQuestionDto>
    {
        public CreateBlankSubQuestionDtoValidator(IServiceProvider sp)
        {
            Include(sp.GetRequiredService<CreateSubQuestionDtoValidator>());
            RuleFor(d => d.Checker)
                .InjectValidator()
                .When(d => d.Checker != null);
            RuleFor(d => d.Answer)
                .MinimumLength(1)
                .When(d => d.Checker == null)
                .WithMessage($"{nameof(CreateBlankSubQuestionDto.Answer)} and {nameof(CreateBlankSubQuestionDto.Checker)} can't be both null.");
        }
    }
}
