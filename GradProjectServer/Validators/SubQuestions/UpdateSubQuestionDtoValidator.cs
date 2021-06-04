using System.Linq;
using FluentValidation;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Validators.SubQuestions
{
    public class UpdateSubQuestionDtoValidator : AbstractValidator<UpdateSubQuestionDto>
    {
        public UpdateSubQuestionDtoValidator(AppDbContext dbContext, IHttpContextAccessor httpContext)
        {
            RuleFor(d => d.Id)
                .MustAsync(async (id, _) =>
                {
                    var user = httpContext.HttpContext!.GetUser();
                    if (user == null)
                    {
                        return false;
                    }

                    var subQuestion = await dbContext.SubQuestions
                        .Include(q => q.Question)
                        .FirstOrDefaultAsync(q => q.Id == id)
                        .ConfigureAwait(false);
                    if (subQuestion == null)
                    {
                        return false;
                    }

                    if (user.IsAdmin)
                    {
                        return true;
                    }

                    return !subQuestion.Question.IsApproved && subQuestion.Question.VolunteerId == user.Id;
                })
                .WithMessage(
                    "SubQuestion(Id: {PropertyValue}) doesn't exist or it's approved or the it doesn't belong to user");
            RuleFor(d => d.Content)
                .NotEmpty()
                .When(d => d.Content != null);
            RuleFor(d => d.TagsToAdd)
                .NotEmpty()
                .Must(ids => ids.Distinct().Count() == ids.Length)
                .WithMessage("{PropertyName} can't contain duplicates")
                .ForEach(b =>
                {
                    b.MustAsync(async (id, _) => (await dbContext.Tags.FindAsync(id).ConfigureAwait(false)) != null)
                        .WithMessage("Tag(Id: {PropertyValue}) doesn't exist");
                })
                .When(d => d.TagsToAdd != null);
            RuleFor(d => d.TagsToDelete)
                .NotEmpty()
                .Must(ids => ids.Distinct().Count() == ids.Length)
                .WithMessage("{PropertyName} can't contain duplicates")
                .MustAsync(async (d, ids, _) =>
                {
                    var subQuestion = await dbContext.SubQuestions
                        .Include(q => q.Tags)
                        .FirstOrDefaultAsync(q => q.Id == d.Id)
                        .ConfigureAwait(false);
                    return !subQuestion.Tags.Select(t => t.TagId).Except(ids).Any();
                })
                .WithMessage("{PropertyName} contains tags that don't belong to SubQuestion(Id: {PropertyValue})");

        }
    }
}