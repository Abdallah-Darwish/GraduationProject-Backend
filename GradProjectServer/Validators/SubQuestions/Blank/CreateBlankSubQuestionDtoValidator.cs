using FluentValidation;
using GradProjectServer.Common;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.FilesManagers;

namespace GradProjectServer.Validators.SubQuestions.Blank
{
    public class CreateBlankSubQuestionDtoValidator : AbstractValidator<CreateBlankSubQuestionDto>
    {
        public CreateBlankSubQuestionDtoValidator(CreateSubQuestionDtoValidator baseValidator, BlankSubQuestionFileManager fileManager)
        {
            Include(baseValidator);
            RuleFor(d => d.Answer)
                .NotEmpty();
            RuleFor(d => d.CheckerBase64)
                .MustAsync(async (c, _) =>
                {
                    await using var checkerStream = await Utility.DecodeBase64Async(c).ConfigureAwait(false);
                    return fileManager.VerifyCheckerSource(checkerStream);
                })
                .WithMessage("Invalid checker source.");
        }
    }
}