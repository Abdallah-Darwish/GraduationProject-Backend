using FluentValidation;
using GradProjectServer.DTO.Questions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.Questions
{
    public class UpdateQuestionDtoValidator : AbstractValidator<UpdateQuestionDto>
    {
        public UpdateQuestionDtoValidator(AppDbContext dbContext, IHttpContextAccessor httpContext)
        {
            RuleFor(d => d.QuestionId)
                .MustAsync(async (id, _) => (await dbContext.Questions.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage(d => "Question(Id: {PropertyValue}) doesn't exist.")
                .DependentRules(() =>
                {
                    RuleFor(d => d.QuestionId)
                    .CustomAsync(async (id, ctx, _) =>
                    {
                        var user = httpContext.HttpContext!.GetUser()!;
                        var question = await dbContext.Questions.FindAsync(id).ConfigureAwait(false);
                        if (!user.IsAdmin)
                        {
                            if (question.VolunteerId != user.Id)
                            {
                                ctx.AddFailure(nameof(UpdateQuestionDto.QuestionId), $"The user doesn't own the Question(Id: {id}).");
                                return;
                            }
                            if (question.IsApproved)
                            {
                                ctx.AddFailure(nameof(UpdateQuestionDto.QuestionId), $"The user can't update the Question(Id: {id}) because its already approved, only admins can update it now.");
                                return;
                            }
                        }
                    });
                    RuleFor(d => d.Content)
                        .NotEmpty()
                        .When(d => d.Content != null);
                    RuleFor(d => d.Title)
                        .NotEmpty()
                        .When(d => d.Title != null);
                    RuleFor(d => d.CourseId)
                        .MustAsync(async (id, _) => (await dbContext.Courses.FindAsync(id).ConfigureAwait(false)) != null)
                        .WithMessage("Course(Id: {PropertyValue}) doesn't exist.")
                        .When(d => d.CourseId.HasValue);
                });
        }
    }
}
