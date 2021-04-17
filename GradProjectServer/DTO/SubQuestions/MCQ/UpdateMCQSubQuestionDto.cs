namespace GradProjectServer.DTO.SubQuestions
{
    public class UpdateMCQSubQuestionDto : UpdateSubQuestionDto
    {
        public bool? IsCheckBox { get; set; }
        public CreateMCQSubQuestionChoiceDto[]? ChoicesToAdd { get; set; }
        public int[]? ChoicesToDelete { get; set; }
        public UpdateMCQSubQuestionChoiceDto[]? ChoicesToUpdate { get; set; }
    }
}
