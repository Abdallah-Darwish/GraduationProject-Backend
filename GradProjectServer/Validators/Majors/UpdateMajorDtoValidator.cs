using FluentValidation;
using GradProjectServer.DTO.Majors;
using GradProjectServer.Services.EntityFramework;

namespace GradProjectServer.Validators.Majors
{
    public class UpdateMajorDtoValidator : AbstractValidator<UpdateMajorDto>
    {
        public UpdateMajorDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.Id)
                .MustAsync(async (id, _) => (await dbContext.Majors.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage("Major(Id: {PropertyValue}) doesn't exist.");
            RuleFor(d => d.Name)
                .NotEmpty()
                .When(d => d.Name != null);
        }
    }
}
