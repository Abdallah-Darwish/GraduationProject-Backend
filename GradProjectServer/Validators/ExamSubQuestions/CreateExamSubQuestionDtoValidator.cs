using FluentValidation;
using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Validators.ExamSubQuestions
{
    public class CreateExamSubQuestionDtoValidator : AbstractValidator<CreateExamSubQuestionDto>
    {
        public CreateExamSubQuestionDtoValidator(AppDbContext dbContext, HttpContextAccessor httpContext)
        {
            RuleFor(d => d.Weight)
                .InclusiveBetween(0.0f, 1.0f);
            RuleFor(d => d.ExamQuestionId)
                .MustAsync(async (id, _) =>
                {
                    var user = httpContext.HttpContext!.GetUser();
                    if (user == null)
                    {
                        return false;
                    }

                    var examQuestion = await dbContext.ExamsQuestions
                        .Include(q => q.Exam)
                        .FirstOrDefaultAsync(q => q.Id == id)
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
                    "ExamQuestion(Id: {PropertyValue}) doesn't exist or the exam is approved or the exam is not owned by the user.");
            RuleFor(d => d.SubQuestionId)
                .MustAsync(async (d, id, _) =>
                {
                    var examQuestion = await dbContext.ExamsQuestions.FindAsync(d.ExamQuestionId).ConfigureAwait(false);
                    var subQuestion = await dbContext.SubQuestions.FindAsync(id).ConfigureAwait(false);
                    if (subQuestion == null)
                    {
                        return false;
                    }

                    return subQuestion.QuestionId == examQuestion.QuestionId;
                })
                .WithMessage(d =>
                    $"SubQuestion(Id: {{PropertyValue}}) doesn't exist or doesn't belong to the original question of ExamQuestion(Id: {d.ExamQuestionId}).");
        }
    }
}