using FluentValidation;
using GradProjectServer.DTO.Programs;
using GradProjectServer.Services.EntityFramework;
using System;
using System.IO;
using System.IO.Compression;

namespace GradProjectServer.Validators.Programs
{
    public class CreateProgramDtoValidator : AbstractValidator<CreateProgramDto>
    {
        static string[]? ValidateChecker(string base64)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64);
                using var archiveStream = new MemoryStream(bytes);
                using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Read, false);
                //todo: check if needed files are provided
            }
            catch
            {
                return new string[] { "Invalid Archive" };
            }
            return null;
        }
        public CreateProgramDtoValidator(AppDbContext dbContext)
        {
            RuleFor(d => d.ArchiveBase64)
                .Custom((base64, ctx) =>
                {
                    var errors = ValidateChecker(base64);
                    if ((errors?.Length ?? 0) == 0) { return; }
                    foreach (var e in errors!)
                    {
                        ctx.AddFailure(e);
                    }
                });

        }
    }
}
