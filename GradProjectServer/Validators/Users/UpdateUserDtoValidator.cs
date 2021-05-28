using FluentValidation;
using GradProjectServer.Common;
using GradProjectServer.DTO.Users;
using GradProjectServer.Services.EntityFramework;
using GradProjectServer.Services.FilesManagers;
using GradProjectServer.Services.UserSystem;
using Microsoft.AspNetCore.Http;

namespace GradProjectServer.Validators.Users
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator(AppDbContext dbContext, IHttpContextAccessor httpContext, UserFileManager userFileManager)
        {
            RuleFor(d => d.Id)
                .MustAsync(async (id, _) => (await dbContext.Users.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage("User(Id: {PropertyValue}) doesn't exist.")
                .MustAsync(async (id, _) =>
                {
                    var caller = httpContext.HttpContext!.GetUser()!;
                    var target = await dbContext.Users.FindAsync(id).ConfigureAwait(false);
                    return caller.Id == id || caller.IsAdmin && !target.IsAdmin;
                });
            RuleFor(d => d.IsAdmin)
                .Must(_ => httpContext.HttpContext!.GetUser()!.IsAdmin)
                .WithMessage("{PropertyName} can't have value unless caller is an admin.")
                .When(d => d.IsAdmin.HasValue);
            RuleFor(d => d.Name)
                .NotEmpty()
                .When(d => d.Name != null);
            RuleFor(d => d.Password)
                .SetValidator(new PasswordValidator<UpdateUserDto>())
                .When(d => d.Password != null);
            RuleFor(d => d.StudyPlanId)
                .MustAsync(async (id, _) => (await dbContext.StudyPlans.FindAsync(id).ConfigureAwait(false)) != null)
                .WithMessage("StudyPlan(Id: {PropertyValue}) doesn't exist")
                .When(d=>d.StudyPlanId.HasValue);
            RuleFor(d => d.ProfilePictureJpgBase64)
                .MustAsync(async (base64, _) =>
                {
                    await using var imageStream =await Utility.DecodeBase64Async(base64!).ConfigureAwait(false);
                    return userFileManager.ValidateProfilePicture(imageStream);
                })
                .When(d => d.ProfilePictureJpgBase64 != null)
                .WithMessage("{PropertyName} isn't a valid Base64 image.")
                .When(d => d.ProfilePictureJpgBase64 != null);
        }
    }
}