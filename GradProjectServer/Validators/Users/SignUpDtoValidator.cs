using System.Linq;
using FluentValidation;
using GradProjectServer.Common;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.FilesManagers;
using Microsoft.EntityFrameworkCore;

namespace GradProjectServer.Validators.Users
{
    public class SignUpDtoValidator : AbstractValidator<SignUpDto>
    {
        
        public SignUpDtoValidator(AppDbContext dbContext, UserFileManager userFileManager)
        {
            RuleFor(d => d.Password)
                .SetValidator(new PasswordValidator<SignUpDto>());
                    
            RuleFor(d => d.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("{PropertyName} is invalid email address")
                .MustAsync(async (e, _) =>
                {
                    e = e.ToLowerInvariant();
                    return !(await dbContext.Users.AnyAsync(u => u.Email.ToLower() == e).ConfigureAwait(false));
                })
                .WithMessage("Email({PropertyValue}) is already signed.");
            RuleFor(d => d.Name)
                .NotEmpty();
            RuleFor(d => d.StudyPlanId)
                .MustAsync(async (id, _) => (await dbContext.StudyPlans.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage("StudyPlan(Id: {PropertyValue}) doesn't exist");
            RuleFor(d => d.ProfilePictureJpgBase64)
                .MustAsync(async (base64, _) =>
                {
                    await using var imageStream =await Utility.DecodeBase64Async(base64!).ConfigureAwait(false);
                    return userFileManager.ValidateProfilePicture(imageStream);
                })
                .When(d => d.ProfilePictureJpgBase64 != null)
                .WithMessage("{PropertyName} isn't a valid Base64 image.");
        }
    }
}