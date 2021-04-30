namespace GradProjectServer.DTO.StudyPlanCourseCategories
{
    public class CreateStudyPlanCourseCategoryDto
    {
        public int StudyPlanId { get; set; }
        public int CategoryId { get; set; }
        public int AllowedCreditHours { get; set; }
    }
}
