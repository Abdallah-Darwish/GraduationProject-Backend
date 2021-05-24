using GradProjectServer.DTO.SubQuestions;

namespace GradProjectServer.DTO.ExamAttempts.SubQuestionGrade
{
    public class MCQSubQuestionGradeDto : SubQuestionGradeDto
    {
        /// <summary>
        /// Returns "Owned" variant so you would know the weight of each choice
        /// </summary>
        public OwnedMCQSubQuestionChoiceDto[] CorrectChoices { get; set; }
        public OwnedMCQSubQuestionChoiceDto[] UserChoices { get; set; }
    }
}