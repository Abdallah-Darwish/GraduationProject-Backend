namespace GradProjectServer.DTO.ExamQuestions
{
    public class CreateExamQuestionDto
    {
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int Order { get; set; }
    }
}