using FluentValidation;
using GradProjectServer.Common;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.Services.FilesManagers;

namespace GradProjectServer.Validators.SubQuestions.Programming
{
    public class CreateProgrammingSubQuestionDtoValidator : AbstractValidator<CreateProgrammingSubQuestionDto>
    {
        public CreateProgrammingSubQuestionDtoValidator(CreateSubQuestionDtoValidator baseValidator, ProgrammingSubQuestionFileManager fileManager)
        {
            Include(baseValidator);
            RuleFor(d => d.CheckerBase64)
                .NotEmpty()
                .MustAsync(async (c, _) =>
                {
                    await using var checkerStream = await Utility.DecodeBase64Async(c).ConfigureAwait(false);
                    return fileManager.VerifyCheckerSource(checkerStream);
                })
                .WithMessage("Invalid checker source.");
            RuleFor(d => d.KeyAnswer)
                .NotNull();
        }
    }
}