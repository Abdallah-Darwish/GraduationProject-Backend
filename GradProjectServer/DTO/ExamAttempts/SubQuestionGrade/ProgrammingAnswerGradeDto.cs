using GradProjectServer.Common;

namespace GradProjectServer.DTO.ExamAttempts.SubQuestionGrade
{
    /// <summary>
    /// To get your answer call SubQuestionAnswerController with the exam sub question id even after finishing the attempt.
    /// </summary>
    public class ProgrammingAnswerGradeDto : AnswerGradeDto
    {
        public ProgrammingSubQuestionAnswerVerdict Verdict { get; set; }
    }
}