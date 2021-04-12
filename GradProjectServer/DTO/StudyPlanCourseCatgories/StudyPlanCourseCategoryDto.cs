using GradProjectServer.DTO.CourseCategories;

namespace GradProjectServer.DTO.StudyPlanCourseCategories
{
    public class StudyPlanCourseCategoryDto
    {
        public int Id { get; set; }
        public int AllowedCreditHours { get; set; }
        public int StudyPlanId { get; set; }
        public CourseCatgoryDto Category { get; set; }
    }
}
