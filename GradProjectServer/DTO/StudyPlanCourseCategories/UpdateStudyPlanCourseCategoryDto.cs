namespace GradProjectServer.DTO.StudyPlanCourseCategories
{
    public class UpdateStudyPlanCourseCategoryDto
    {
        public int Id { get; set; }
        public int? StudyPlanId { get; set; }
        public int? CategoryId { get; set; }
        public int? AllowedCreditHours { get; set; }
    }
}