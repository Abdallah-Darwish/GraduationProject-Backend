using FluentValidation;
using GradProjectServer.DTO.Tags;

namespace GradProjectServer.Validators.Tags
{
    public class CreateTagDtoValidator : AbstractValidator<CreateTagDto>
    {
        public CreateTagDtoValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty();
        }
    }
}
