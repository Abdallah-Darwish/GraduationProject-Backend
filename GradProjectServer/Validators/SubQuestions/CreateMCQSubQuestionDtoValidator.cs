using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.SubQuestions
{
    public class CreateMCQSubQuestionDtoValidator: AbstractValidator<CreateMCQSubQuestionDto>
    {
        public CreateMCQSubQuestionDtoValidator(IServiceProvider sp)
        {
            Include(sp.GetRequiredService<CreateSubQuestionDtoValidator>());
            RuleForEach(d => d.Choices)
                .SetValidator(sp.GetRequiredService<CreateMCQSubQuestionChoiceDtoValidator>());
        }
    }
}
