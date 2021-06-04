using FluentValidation;
using GradProjectServer.Common;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.FilesManagers;

namespace GradProjectServer.Validators.SubQuestions.Blank
{
    public class UpdateBlankSubQuestionDtoValidator : AbstractValidator<UpdateBlankSubQuestionDto>
    {
        public UpdateBlankSubQuestionDtoValidator(UpdateSubQuestionDtoValidator baseValidator, BlankSubQuestionFileManager fileManager)
        {
            Include(baseValidator);
            RuleFor(d => d.Answer)
                .NotEmpty()
                .When(d => d.Answer != null);
            RuleFor(d => d.CheckerBase64)
                .NotEmpty()
                .MustAsync(async (c, _) =>
                {
                    await using var checkerStream = await Utility.DecodeBase64Async(c).ConfigureAwait(false);
                    return fileManager.VerifyCheckerSource(checkerStream);
                })
                .WithMessage("Invalid checker source.")
                .When(d => d.UpdateChecker);
        }
    }
}