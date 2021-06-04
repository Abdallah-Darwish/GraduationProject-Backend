using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.SubQuestions
{
    public class UpdateBlankSubQuestionDto : UpdateSubQuestionDto
    {
        public string? CheckerBase64 { get; set; }
        public string? Answer { get; set; }

        /// <summary>
        /// Because you might want to clear <see cref="OwnedBlankSubQuestionDto.Checker"/> so you set it to null.
        /// </summary>
        public bool UpdateChecker { get; set; }
    }
}