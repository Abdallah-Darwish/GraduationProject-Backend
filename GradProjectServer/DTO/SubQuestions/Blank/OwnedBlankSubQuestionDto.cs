using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.SubQuestions
{
    public class OwnedBlankSubQuestionDto : SubQuestionDto
    {
        public string? Answer { get; set; }
        public ProgramDto? Checker { get; set; }
    }
}
