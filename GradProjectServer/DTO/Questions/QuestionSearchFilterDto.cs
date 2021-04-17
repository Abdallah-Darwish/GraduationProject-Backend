namespace GradProjectServer.DTO.Questions
{
    public class QuestionSearchFilterDto
    {
        public int[]? QuestionsIds { get; set; }
        public int[]? TagsIds { get; set; }
        public int[]? CoursesIds { get; set; }
        public string? TitleMask { get; set; }
        //useful for user page
        public bool? IsApproved { get; set; }
        //useful for admins and user page
        public int[]? VolunteersIds { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
        public bool Metadata { get; set; }
    }
}
