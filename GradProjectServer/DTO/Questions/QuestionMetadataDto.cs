using GradProjectServer.DTO.Courses;

namespace GradProjectServer.DTO.Questions
{
    public class QuestionMetadataDto
    {
        public int Id { get; set; }
        public CourseDto Course { get; set; }
        public string Title { get; set; }
        public bool IsApproved { get; set; }
    }
}