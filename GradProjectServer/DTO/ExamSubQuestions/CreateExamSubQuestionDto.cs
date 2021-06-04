namespace GradProjectServer.DTO.ExamSubQuestions
{
    public class CreateExamSubQuestionDto
    {
        public int ExamQuestionId { get; set; }
        public int SubQuestionId { get; set; }
        public float Weight { get; set; }
        public int Order { get; set; }
    }
}