using GradProjectServer.DTO.SubQuestions;

namespace GradProjectServer.DTO.ExamSubQuestions
{
    public class ExamSubQuestionDto
    {
        public int Id { get; set; }
        public SubQuestionMetadataDto SubQuestion { get; set; }
        public float Weight { get; set; }
        public int Order { get; set; }
    }
}