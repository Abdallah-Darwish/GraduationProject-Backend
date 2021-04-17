using FluentValidation;
using GradProjectServer.DTO.Exams;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using System;

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
                .WithMessage("Exam(Id: {PropertyName}) doesn't exist.")
                .DependentRules(() =>
                {
                    RuleFor(d => d.ExamId)
                    .CustomAsync(async (id, ctx, _) =>
                    {
                        var exam = await dbContext.Exams.FindAsync(id).ConfigureAwait(false);
                        var user = httpContext.HttpContext!.GetUser()!;
                        if (!user.IsAdmin)
                        {
                            if (exam.VolunteerId != user.Id)
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
                .NotEmpty()
                .When(d => d.Name != null);
            RuleFor(d => d.Duration)
                .InclusiveBetween(TimeSpan.FromSeconds(1), TimeSpan.FromHours(10))
                .WithMessage("{PropertyName} must be in range [1 second, 10 hours].")
                .When(d => d.Duration.HasValue);
            RuleFor(d => d.Year)
                .InclusiveBetween(1990, DateTime.Now.Year)
                .WithMessage("{PropertyName} must be in range [1990, Current Year].")
                .When(d => d.Year.HasValue);
            RuleFor(d => d.Type)
                .IsInEnum()
                .When(d => d.Type.HasValue);
            RuleFor(d => d.Semester)
                .IsInEnum()
                .When(d => d.Type.HasValue);
        }
    }
}
