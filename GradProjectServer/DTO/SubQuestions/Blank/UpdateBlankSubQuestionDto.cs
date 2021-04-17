using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.SubQuestions
{
    public class UpdateBlankSubQuestionDto : UpdateSubQuestionDto
    {
        public CreateProgramDto? Checker { get; set; }
        public string? Answer { get; set; }
        /// <summary>
        /// Because you might want to clear <see cref="OwnedBlankSubQuestionDto.Checker"/> so you set it to null.
        /// </summary>
        public bool UpdateChecker { get; set; }
        public bool UpdateAnswer { get; set; }

    }
}
