namespace GradProjectServer.DTO.SubQuestions
{
    public class OwnedMCQSubQuestionDto : SubQuestionDto
    {
        public bool IsCheckBox { get; set; }
        public OwnedMCQSubQuestionChoiceDto[] Choices { get; set; }
    }
}
