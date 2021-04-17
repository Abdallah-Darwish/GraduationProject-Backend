using GradProjectServer.DTO.ExamSubQuestions;

namespace GradProjectServer.DTO.ExamQuestions
{
    public class ExamQuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public int QuestionId { get; set; }
        public ExamSubQuestionDto[] ExamSubQuestions { get; set; }
    }
}
