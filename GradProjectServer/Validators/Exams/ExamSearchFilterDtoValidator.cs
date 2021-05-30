using FluentValidation;
using GradProjectServer.DTO.Exams;

namespace GradProjectServer.Validators.Exams
{
    public class ExamSearchFilterDtoValidator : AbstractValidator<ExamSearchFilterDto>
    {
        public ExamSearchFilterDtoValidator()
        {
            RuleFor(d => d.Count)
                .GreaterThan(0)
                .WithMessage("{PropertyName} must be >= {ComparisonValue}.");
            RuleFor(d => d.Offset)
                .GreaterThanOrEqualTo(0)
                .WithMessage("{PropertyName} must be >= {ComparisonValue}.");
            RuleFor(d => d.MinDuration)
                .GreaterThan(0)
                .When(d=> d.MinDuration.HasValue)
                .LessThan(d => d.MaxDuration)
                .When(d => d.MinDuration.HasValue && d.MaxDuration.HasValue)
                .WithMessage(
                    $"{nameof(ExamSearchFilterDto.MinDuration)} must be <= ${nameof(ExamSearchFilterDto.MaxDuration)}.");
            RuleFor(d => d.MaxDuration)
                .GreaterThan(0)
                .When(d => d.MaxDuration.HasValue);
            RuleFor(d => d.MinYear)
                .GreaterThan(0)
                .When(d=> d.MinYear.HasValue)
                .LessThan(d => d.MaxYear)
                .When(d => d.MinYear.HasValue && d.MaxYear.HasValue)
                .WithMessage(
                    $"{nameof(ExamSearchFilterDto.MinYear)} must be <= ${nameof(ExamSearchFilterDto.MaxYear)}.");
            RuleFor(d => d.MaxYear)
                .GreaterThan(0)
                .When(d => d.MaxYear.HasValue);
        }
    }
}