using FluentValidation;
using GradProjectServer.DTO.Tags;

namespace GradProjectServer.Validators.Tags
{
    public class TagSearchFilterDtoValidator : AbstractValidator<TagSearchFilterDto>
    {
        public TagSearchFilterDtoValidator()
        {
            RuleFor(d => d.Count)
                .GreaterThan(0);
            RuleFor(d => d.Offset)
                .GreaterThanOrEqualTo(0);
            RuleFor(d => d.NameMask)
                .NotEmpty()
                .When(d => d.NameMask != null);
        }
    }
}