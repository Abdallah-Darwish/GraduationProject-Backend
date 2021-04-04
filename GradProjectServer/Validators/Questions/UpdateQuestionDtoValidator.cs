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
                .WithMessage(d => $"Question(Id: {d.QuestionId}) doesn't exist.")
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
                        .MinimumLength(1)
                        .WithMessage($"{nameof(UpdateQuestionDto.Content)} can't be null or empty.")
                        .When(d => d.Content != null);
                    RuleFor(d => d.Title)
                        .MinimumLength(1)
                        .WithMessage($"{nameof(UpdateQuestionDto.Title)} can't be null or empty.")
                        .When(d => d.Title != null);
                    RuleFor(d => d.CourseId)
                        .MustAsync(async (id, _) => (await dbContext.Courses.FindAsync(id).ConfigureAwait(false)) != null)
                        .WithMessage(d => $"Course(Id: {d.CourseId}) doesn't exist.")
                        .When(d => d.CourseId.HasValue);
                    RuleFor(d => d.SubQuestionsToDelete)
                        .Must(questions => questions!.Length >= 1)
                        .WithMessage($"{nameof(UpdateQuestionDto.SubQuestionsToDelete)} size must be >= 1.")
                        .Must(questions => questions!.Distinct().Count() == questions!.Length)
                        .WithMessage($"{nameof(UpdateQuestionDto.SubQuestionsToDelete)} can't contain duplicates.")
                        .MustAsync(async (d, questionsToDelete, _) =>
                        {
                            var question = await dbContext.Questions.FindAsync(d.QuestionId).ConfigureAwait(false);
                            if (question == null) { return true; }
                            var nonExistingQuestions = question.SubQuestions
                            .Select(q => q.Id)
                            .Except(questionsToDelete!)
                            .ToArray();
                            if (nonExistingQuestions.Length == 0) { return true; }
                            return false;
                        })
                        .WithMessage(d => $"Some questions in {nameof(UpdateQuestionDto.SubQuestionsToDelete)} don't exist or don't belong to Question(Id: {d.QuestionId}).")
                        .When(d => d.SubQuestionsToDelete != null);
                });
        }
    }
}
