using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.SubQuestions
{
    public class CreateBlankSubQuestionDto : CreateSubQuestionDto
    {
        public string? CheckerBase64 { get; set; }
        public string Answer { get; set; }
    }
}