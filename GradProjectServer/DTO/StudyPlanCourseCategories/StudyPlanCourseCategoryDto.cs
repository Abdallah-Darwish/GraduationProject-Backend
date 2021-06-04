using GradProjectServer.DTO.CourseCategories;
using GradProjectServer.DTO.StudyPlanCourses;
using GradProjectServer.DTO.StudyPlans;

namespace GradProjectServer.DTO.StudyPlanCourseCategories
{
    public class StudyPlanCourseCategoryDto
    {
        public int Id { get; set; }
        public int AllowedCreditHours { get; set; }
        public int StudyPlanId { get; set; }
        public CourseCategoryDto Category { get; set; }
        public StudyPlanCourseDto[] Courses { get; set; }
    }
}