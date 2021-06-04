using GradProjectServer.DTO.ExamAttempts.SubQuestionGrade;

namespace GradProjectServer.DTO.ExamAttempts
{
    public class ExamAttemptGradeDto
    {
        public float Grade { get; set; }
        public float Weight { get; set; }
        public AnswerGradeDto[] AnswersGrades { get; set; }
    }
}