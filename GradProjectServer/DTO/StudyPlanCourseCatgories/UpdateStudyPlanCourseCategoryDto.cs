namespace GradProjectServer.DTO.StudyPlanCourseCategories
{
    public class UpdateStudyPlanCourseCategoryDto
    {
        public int Id { get; set; }
        public int? MajorId { get; set; }
        public int? CategoryId { get; set; }
        public int? AllowedCreditHoures { get; set; }
        public int[]? CoursesToAdd { get; set; }
        public int[]? CoursesToDelete { get; set; }
    }
}
