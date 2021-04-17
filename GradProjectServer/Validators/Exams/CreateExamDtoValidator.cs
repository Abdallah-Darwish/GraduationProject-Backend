using FluentValidation;
using GradProjectServer.DTO.Exams;
using GradProjectServer.Services.EntityFramework;
using System;

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
            RuleFor(d => d.Type)
                .IsInEnum();
            RuleFor(d => d.Semester)
                .IsInEnum();
        }
    }
}
