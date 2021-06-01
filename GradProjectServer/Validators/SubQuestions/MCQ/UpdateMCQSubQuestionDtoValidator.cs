using System;
using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace GradProjectServer.Validators.SubQuestions.MCQ
{
    public class UpdateMCQSubQuestionDtoValidator : AbstractValidator<UpdateMCQSubQuestionDto>
    {
        public UpdateMCQSubQuestionDtoValidator(UpdateSubQuestionDtoValidator baseValidator, AppDbContext dbContext, IServiceProvider sp)
        {
            Include(baseValidator);
            RuleFor(d => d.ChoicesToDelete)
                .NotEmpty()
                .When(d => d.ChoicesToDelete != null);
            RuleFor(d => d.ChoicesToAdd)
                .NotEmpty()
                .When(d => d.ChoicesToAdd != null);
            RuleFor(d => d.ChoicesToUpdate)
                .NotEmpty()
                .When(d => d.ChoicesToUpdate != null);
            RuleForEach(d => d.ChoicesToDelete)
                .MustAsync(async (d, id, _) =>
                {
                    var choice = await dbContext.MCQSubQuestionsChoices.FindAsync(id).ConfigureAwait(false);
                    return choice?.SubQuestionId == d.Id;
                })
                .WithMessage(d =>
                    $"Some choice to delete don't exist or don't belong to MCQSubQuestion(Id: {d.Id}")
                .When(d=>d.ChoicesToDelete != null);
            RuleForEach(d=>d.ChoicesToAdd)
                .SetValidator(sp.GetRequiredService<CreateMCQSubQuestionChoiceDtoValidator>())
                .When(d=>d.ChoicesToAdd != null);
            RuleForEach(d=>d.ChoicesToUpdate)
                .MustAsync(async (d, id, _) =>
                {
                    var choice = await dbContext.MCQSubQuestionsChoices.FindAsync(id).ConfigureAwait(false);
                    return choice?.SubQuestionId == d.Id;
                })
                .WithMessage(d =>
                    $"Some choice to update don't exist or don't belong to MCQSubQuestion(Id: {d.Id}")
                .SetValidator(sp.GetRequiredService<UpdateMCQSubQuestionChoiceDtoValidator>())
                .When(d=>d.ChoicesToUpdate != null);
        }
    }
}