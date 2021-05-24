using GradProjectServer.DTO.ExamSubQuestions;

namespace GradProjectServer.DTO.ExamAttempts.SubQuestionGrade
{
    public class AnswerGradeDto
    {
        public ExamSubQuestionDto SubQuestion { get; set; }
        public string? Comment { get; set; }
        public float Grade { get; set; }
        public float Weight { get; set; }
    }
}