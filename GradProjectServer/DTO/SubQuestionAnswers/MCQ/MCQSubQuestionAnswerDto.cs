using GradProjectServer.DTO.SubQuestions;

namespace GradProjectServer.DTO.SubQuestionAnswers.MCQ
{
    public class MCQSubQuestionAnswerDto : SubQuestionAnswerDto
    {
        public MCQSubQuestionChoiceDto[] SelectedChoices { get; set; }
    }
}