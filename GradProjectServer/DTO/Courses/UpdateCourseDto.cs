namespace GradProjectServer.DTO.Courses
{
    public class UpdateCourseDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? CreditHours { get; set; }
    }
}