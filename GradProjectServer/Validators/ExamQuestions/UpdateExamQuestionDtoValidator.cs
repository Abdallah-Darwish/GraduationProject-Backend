using FluentValidation;
using GradProjectServer.DTO.ExamQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Validators.ExamQuestions
{
    public class UpdateExamQuestionDtoValidator : AbstractValidator<UpdateExamQuestionDto>
    {
        public UpdateExamQuestionDtoValidator(AppDbContext dbContext, HttpContextAccessor httpContext)
        {
            RuleFor(d => d.Id)
                .MustAsync(async (id, _) =>
                {
                    var user = httpContext.HttpContext!.GetUser();
                    if (user == null)
                    {
                        return false;
                    }

                    var examQuestion = await dbContext.ExamsQuestions
                        .Include(e => e.Exam)
                        .FirstOrDefaultAsync(e => e.Id == id)
                        .ConfigureAwait(false);
                    if (examQuestion == null)
                    {
                        return false;
                    }

                    if (user.IsAdmin)
                    {
                        return true;
                    }

                    return !examQuestion.Exam.IsApproved && examQuestion.Exam.VolunteerId == user.Id;
                })
                .WithMessage(
                    "ExamQuestion(Id: {PropertyValue}) doesn't exist or is already approved or isn't owned by user.");
        }
    }
}