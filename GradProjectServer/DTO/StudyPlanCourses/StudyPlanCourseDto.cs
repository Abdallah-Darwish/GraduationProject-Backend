using GradProjectServer.DTO.Courses;

namespace GradProjectServer.DTO.StudyPlanCourses
{
    public class StudyPlanCourseDto
    {
        public int StudyPlanCourseCategoryId { get; set; }
        public CourseDto Course { get; set; }
        public StudyPlanCoursePrerequisiteDto[] Prerequisites { get; set; }
    }
}