namespace GradProjectServer.DTO.Questions
{
    public class CreateQuestionDto
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public int CourseId { get; set; }
    }
}