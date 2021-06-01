namespace GradProjectServer.DTO.SubQuestions
{
    public class UpdateSubQuestionDto
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public int[]? TagsToDelete { get; set; }
        public int[]? TagsToAdd { get; set; }
        //can't update QuestionId because of approval and ownership issues.
    }
}