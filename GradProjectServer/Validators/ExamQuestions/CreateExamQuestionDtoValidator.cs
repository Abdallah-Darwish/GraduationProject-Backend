using FluentValidation;
using GradProjectServer.DTO.ExamQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;

namespace GradProjectServer.Validators.ExamQuestions
{
    public class CreateExamQuestionDtoValidator : AbstractValidator<CreateExamQuestionDto>
    {
        public CreateExamQuestionDtoValidator(AppDbContext dbContext, IHttpContextAccessor httpContext)
        {
            RuleFor(d => d.ExamId)
                .MustAsync(async (id, _) =>
                {
                    var user = httpContext.HttpContext!.GetUser();
                    if (user == null)
                    {
                        return false;
                    }

                    var exam = await dbContext.Exams.FindAsync(id).ConfigureAwait(false);
                    if (exam == null)
                    {
                        return false;
                    }

                    if (user.IsAdmin)
                    {
                        return true;
                    }
                    return !exam.IsApproved && exam.VolunteerId == user.Id;
                })
                .WithMessage("Exam(Id: {PropertyValue}) doesn't exist or approved or not owned by caller.");
            RuleFor(d => d.QuestionId)
                .MustAsync(async (d, id, _) =>
                {
                    var question = await dbContext.Questions.FindAsync(id).ConfigureAwait(false);
                    var exam = await dbContext.Exams.FindAsync(d.ExamId).ConfigureAwait(false);
                    return (question?.IsApproved ?? false) && question.CourseId == exam.CourseId;
                })
                .WithMessage(
                    "Question(Id: {PropertyValue}) doesn't exist or not approved or doesn't belong to same course of Exam(Id: {PropertyValue}).");
            
        }
    }
}