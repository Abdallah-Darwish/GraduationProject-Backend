namespace GradProjectServer.DTO.CourseCategories
{
    public class CourseCategorySearchFilterDto
    {
        public string? NameMask { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}