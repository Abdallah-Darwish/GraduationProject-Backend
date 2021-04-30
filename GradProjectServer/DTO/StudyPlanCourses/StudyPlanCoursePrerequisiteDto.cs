using GradProjectServer.DTO.Courses;

namespace GradProjectServer.DTO.StudyPlanCourses
{
    public class StudyPlanCoursePrerequisiteDto
    {
        public int StudyPlanCourseId { get; set; }

        public CourseDto Course { get; set; }
    }
}