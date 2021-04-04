using FluentValidation;
using GradProjectServer.DTO.Exams;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.Exams
{
    public class UpdateExamDtoValidator : AbstractValidator<UpdateExamDto>
    {
        public UpdateExamDtoValidator(AppDbContext dbContext, IHttpContextAccessor httpContext)
        {
            //todo: set cascade mode for all validators
            CascadeMode = CascadeMode.Stop;
            RuleFor(d => d.ExamId)
                .MustAsync(async (id, _) => (await dbContext.Exams.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage(d => $"Exam(Id: {d.ExamId}) doesn't exist.")
                .DependentRules(() =>
                {
                    RuleFor(d => d.ExamId)
                    .CustomAsync(async (id,ctx, _) =>
                    {
                        var exam = await dbContext.Exams.FindAsync(id).ConfigureAwait(false);
                        var user = httpContext.HttpContext!.GetUser()!;
                        if (!user.IsAdmin)
                        {
                            if(exam.VolunteerId != user.Id)
                            {
                                ctx.AddFailure(nameof(UpdateExamDto.ExamId), "The user doesn't own the exam.");
                                return;
                            }
                            if (exam.IsApproved)
                            {
                                ctx.AddFailure(nameof(UpdateExamDto.ExamId), "The user can't update the exam because its already approved, only admins can update it now.");
                                return;
                            }
                        }
                    });
                });
            RuleFor(d => d.CourseId)
                .MustAsync(async (id, _) => (await dbContext.Courses.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage(d => $"Course(Id: {d.CourseId}) doesn't exist.")
                .When(d => d.CourseId.HasValue);
            RuleFor(d => d.Name)
                .MinimumLength(1)
                .WithMessage("Exam name can't be empty.")
                .When(d => d.Name != null);
            RuleFor(d => d.Duration)
                .InclusiveBetween(TimeSpan.FromSeconds(1), TimeSpan.FromHours(10))
                .WithMessage("Exam duration must be in range [1 second, 10 hours].")
                .When(d => d.Duration.HasValue);
            RuleFor(d => d.Year)
                .InclusiveBetween(1990, DateTime.Now.Year)
                .WithMessage("Exam year must be in range [1990, Current Year].")
                .When(d => d.Year.HasValue);
            RuleFor(d => d.SubQuestionsToAdd)
                .Must(questions => questions!.Length >= 1)
                .WithMessage($"{nameof(UpdateExamDto.SubQuestionsToAdd)} size must be >= 1.")
                .Must(questions => questions!.Select(q => q.QuestionId).Distinct().Count() == questions!.Length)
                .WithMessage($"{nameof(UpdateExamDto.SubQuestionsToAdd)} can't contain duplicates.")
                .InjectValidator()
                .Must(questions =>
                {
                    var questionsIds = questions!.Select(q => q.QuestionId).ToArray();
                    var questionsCoursesCount = dbContext.Questions
                    .Where(q => questionsIds.Contains(q.Id))
                    .Select(q => q.CourseId)
                    .Distinct()
                    .Count();
                    return questionsCoursesCount == 1;
                })
                .WithMessage($"Not all questions in \"{nameof(UpdateExamDto.SubQuestionsToAdd)}\" belong to the same course.")
                .When(d => d.SubQuestionsToAdd != null);
            RuleFor(d => d.SubQuestionsToDelete)
                .Must(questions => questions!.Length >= 1)
                .WithMessage($"{nameof(UpdateExamDto.SubQuestionsToDelete)} size must be >= 1.")
                .Must(questions => questions!.Distinct().Count() == questions!.Length)
                .WithMessage($"{nameof(UpdateExamDto.SubQuestionsToDelete)} can't contain duplicates.")
                .MustAsync(async (d, questions, _) =>
                {
                    var exam = await dbContext.Exams
                        .FindAsync(d.ExamId)
                        .ConfigureAwait(false);
                    return !questions!.Except(exam.SubQuestions.Select(q => q.Id)).Any();
                })
                .WithMessage($"Some questions in \"{nameof(UpdateExamDto.SubQuestionsToDelete)}\" don't exist or belong to this exam.")
                .When(d => d.SubQuestionsToDelete != null);
            RuleFor(d => d.Type)
                .IsInEnum()
                .When(d => d.Type.HasValue);
            RuleFor(d => d.Semester)
                .IsInEnum()
                .When(d => d.Type.HasValue);
        }
    }
}
