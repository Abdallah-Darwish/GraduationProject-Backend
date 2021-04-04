using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.SubQuestions
{
    public class CreateSubQuestionDtoValidator : AbstractValidator<CreateSubQuestionDto>
    {
        public CreateSubQuestionDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.Content)
                .MinimumLength(1)
                .WithMessage($"{nameof(CreateSubQuestionDto.Content)} can't be null or empty.");
            RuleFor(d => d.Tags)
                .Must(ids => ids!.Distinct().Count() == ids!.Length)
                .WithMessage($"{nameof(CreateSubQuestionDto.Tags)} can't contain duplicates.")
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
