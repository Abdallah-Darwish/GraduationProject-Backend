using GradProjectServer.DTO.ExamSubQuestions;

namespace GradProjectServer.DTO.ExamAttempts.SubQuestionGrade
{
    public class SubQuestionGradeDto
    {
        public ExamSubQuestionDto SubQuestion { get; set; }
        public string Comment { get; set; }
        public double Grade { get; set; }
        public double Weight { get; set; }
    }
}