using FluentValidation;
using GradProjectServer.DTO.Majors;

namespace GradProjectServer.Validators.Majors
{
    public class CreateMajorDtoValidator : AbstractValidator<CreateMajorDto>
    {
        public CreateMajorDtoValidator()
        {
            RuleFor(d => d.Name)
                .NotEmpty();
        }
    }
}