using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.AspNetCore;
using FluentValidation;
using FluentValidation.Validators;
using GradProjectServer.DTO.Exams;
using GradProjectServer.Services.EntityFramework;

namespace GradProjectServer.Validators.Exams
{
	public class CreateExamDtoValidator : AbstractValidator<CreateExamDto>
	{
		public CreateExamDtoValidator(AppDbContext dbContext)
		{
			RuleFor(d => d.Name)
				.NotEmpty();
			RuleFor(d => d.Duration)
				.InclusiveBetween(TimeSpan.FromSeconds(1), TimeSpan.FromHours(10))
				.WithMessage("{PropertyName} must be in range [1 second, 10 hours].");
			RuleFor(d => d.Year)
				.InclusiveBetween(1990, DateTime.Now.Year)
				.WithMessage("{PropertyName} must be in range [1990, Current Year].");
			RuleFor(d => d.SubQuestions)
				.NotNull()
				.WithMessage("{PropertyName} can't be null.")
				.Must(questions => questions.Length >= 1)
				.WithMessage("{PropertyName} size must be >= 1.")
				.InjectValidator()
				.Must(questions =>
				{
					var questionsIds = questions.Select(q => q.QuestionId).ToArray();
					var questionsCoursesCount = dbContext.Questions
					.Where(q => questionsIds.Contains(q.Id))
					.Select(q => q.CourseId)
					.Distinct()
					.Count();
					return questionsCoursesCount == 1;
				})
				.WithMessage("Not all questions in {PropertyName} belong to the same course.");
			RuleFor(d => d.Type)
				.IsInEnum();
			RuleFor(d => d.Semester)
				.IsInEnum();
		}
	}
}
