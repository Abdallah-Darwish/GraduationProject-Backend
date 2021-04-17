using GradProjectServer.DTO.SubQuestions;

namespace GradProjectServer.DTO.ExamSubQuestions
{
    public class ExamSubQuestionDto
    {
        public SubQuestionMetadataDto SubQuestion { get; set; }
        public float Weight { get; set; }
    }
}
