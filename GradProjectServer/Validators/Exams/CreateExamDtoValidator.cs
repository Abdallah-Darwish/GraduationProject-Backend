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
				.MinimumLength(1)
				.WithMessage("Exam name can't be empty.");
			RuleFor(d => d.Duration)
				.InclusiveBetween(TimeSpan.FromSeconds(1), TimeSpan.FromHours(10))
				.WithMessage("Exam duration must be in range [1 second, 10 hours].");
			RuleFor(d => d.Year)
				.InclusiveBetween(1990, DateTime.Now.Year)
				.WithMessage("Exam year must be in range [1990, Current Year].");
			RuleFor(d => d.SubQuestions)
				.NotNull()
				.WithMessage($"{nameof(CreateExamDto.SubQuestions)} can't be null.")
				.Must(questions => questions.Length > 1)
				.WithMessage($"{nameof(CreateExamDto.SubQuestions)} size must be >= 1.")
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
				.WithMessage($"Not all questions in \"{nameof(CreateExamDto.SubQuestions)}\" belong to the same course.");
		}
	}
}
