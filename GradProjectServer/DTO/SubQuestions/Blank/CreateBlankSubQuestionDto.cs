using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.SubQuestions
{
    public class CreateBlankSubQuestionDto : CreateSubQuestionDto
    {
        public CreateProgramDto? Checker { get; set; }
        public string? Answer { get; set; }
    }
}
