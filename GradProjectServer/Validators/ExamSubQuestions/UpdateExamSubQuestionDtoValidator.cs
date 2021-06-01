using FluentValidation;
using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Validators.ExamSubQuestions
{
    public class UpdateExamSubQuestionDtoValidator : AbstractValidator<UpdateExamSubQuestionDto>
    {
        public UpdateExamSubQuestionDtoValidator(AppDbContext dbContext, IHttpContextAccessor httpContext)
        {
            RuleFor(d => d.Weight)
                .InclusiveBetween(0.0f, 1.0f)
                .When(d => d.Weight.HasValue);
            RuleFor(d => d.Id)
                .MustAsync(async (id, _) =>
                {
                    var user = httpContext.HttpContext!.GetUser();
                    if (user == null)
                    {
                        return false;
                    }

                    var examSubQuestion = await dbContext.ExamSubQuestions
                        .Include(q => q.ExamQuestion)
                        .ThenInclude(q => q.Exam)
                        .FirstOrDefaultAsync(q => q.Id == id)
                        .ConfigureAwait(false);
                    if (examSubQuestion == null)
                    {
                        return false;
                    }
                    if (user.IsAdmin)
                    {
                        return true;
                    }
                    return !examSubQuestion.ExamQuestion!.Exam.IsApproved && examSubQuestion.ExamQuestion.Exam.VolunteerId == user.Id;
                })
                .WithMessage(
                    "ExamSubQuestion(Id: {PropertyValue}) doesn't exist or the exam is approved or the exam is not owned by the user.");
        }
    }
}