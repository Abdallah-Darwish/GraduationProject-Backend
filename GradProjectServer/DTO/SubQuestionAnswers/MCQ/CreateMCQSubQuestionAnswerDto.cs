namespace GradProjectServer.DTO.SubQuestionAnswers.MCQ
{
    public class CreateMCQSubQuestionAnswerDto : CreateSubQuestionAnswerDto
    {
        public int[] SelectedChoices { get; set; }
    }
}