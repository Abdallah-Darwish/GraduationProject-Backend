using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.SubQuestions
{
    public class OwnedBlankSubQuestionDto : SubQuestionDto
    {
        public string? Answer { get; set; }
        public bool HasChecker { get; set; }
    }
}