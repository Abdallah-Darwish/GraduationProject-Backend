using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GradProjectServer.Validators.SubQuestions
{
    public class CreateSubQuestionDtoValidator : AbstractValidator<CreateSubQuestionDto>
    {
        public CreateSubQuestionDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.QuestionId)
                .MustAsync(async (id, _) => (await dbContext.Questions.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage("Question(Id: {PropertyValue}) doesn't exist.");
            RuleFor(d => d.Content)
                .NotEmpty();
            RuleFor(d => d.Tags)
                .Must(ids => ids!.Distinct().Count() == ids!.Length)
                .WithMessage("{PropertyName} can't contain duplicates.")
                .When(d => (d.Tags?.Length ?? 0) > 0);
            RuleFor(d => d.Tags)
                .Custom((ids, ctx) =>
                {
                    if ((ids?.Length ?? 0) == 0) { return; }

                    var existingTags = dbContext.Tags
                    .Where(t => ids!.Contains(t.Id))
                    .Select(t => t.Id)
                    .ToArray();
                    var nonExistingTags = ids!.Except(existingTags).ToArray();
                    if (nonExistingTags.Length > 0)
                    {
                        ctx.AddFailure($"There are no tags with the following ids {{ {string.Join(", ", nonExistingTags)} }}.");
                    }
                });
            RuleFor(d => d.Type)
                .IsInEnum();
        }
    }
}
