namespace GradProjectServer.DTO.SubQuestions
{
    public class UpdateSubQuestionDto
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int[]? TagsToDelete { get; set; }
        public int[]? TagsToAdd { get; set; }
        public int? QuestionId { get; set; }
    }
}
