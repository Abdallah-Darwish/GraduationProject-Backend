namespace GradProjectServer.DTO.SubQuestions
{
    public class MCQSubQuestionDto : SubQuestionDto
    {
        public bool IsCheckBox { get; set; }
        public MCQSubQuestionChoiceDto[] Choices { get; set; }
    }
}
