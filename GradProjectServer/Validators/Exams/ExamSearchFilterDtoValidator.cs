using FluentValidation;
using GradProjectServer.DTO.Exams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Validators.Exams
{
    public class ExamSearchFilterDtoValidator : AbstractValidator<ExamSearchFilterDto>
    {
        public ExamSearchFilterDtoValidator()
        {
            RuleFor(d => d.Count)
                .GreaterThan(0)
                .WithMessage($"{nameof(ExamSearchFilterDto.Count)} must be >= 1.");
            RuleFor(d => d.Offset)
                .GreaterThanOrEqualTo(0)
                .WithMessage($"{nameof(ExamSearchFilterDto.Offset)} must be >= 0.");
            RuleFor(d => d.MinDuration)
                .LessThan(d => d.MaxDuration)
                .When(d => d.MinDuration.HasValue && d.MaxDuration.HasValue)
                .WithMessage($"{nameof(ExamSearchFilterDto.MinDuration)} must be <= ${ nameof(ExamSearchFilterDto.MaxDuration)}.");
            RuleFor(d => d.MinYear)
                .LessThan(d => d.MaxYear)
                .When(d => d.MinYear.HasValue && d.MaxYear.HasValue)
                .WithMessage($"{nameof(ExamSearchFilterDto.MinYear)} must be <= ${ nameof(ExamSearchFilterDto.MaxYear)}.");
        }
    }
}
