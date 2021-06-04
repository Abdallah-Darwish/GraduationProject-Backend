using FluentValidation;
using GradProjectServer.DTO.Tags;
using GradProjectServer.Services.EntityFramework;

namespace GradProjectServer.Validators.Tags
{
    public class UpdateTagDtoValidator : AbstractValidator<UpdateTagDto>
    {
        public UpdateTagDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.Id)
                .MustAsync(async (id, _) => (await dbContext.Tags.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage("Tag(Id: {PropertyValue}) doesn't exist.");
            RuleFor(d => d.Name)
                .NotEmpty()
                .When(d => d.Name != null);
        }
    }
}