using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.StudyPlanCourseCategories;

namespace GradProjectServer.DTO.StudyPlans
{
    public class StudyPlanDto : StudyPlanMetadataDto
    {
        public CourseDto[] CoursesData { get; set; }
        public StudyPlanCourseCategoryDto[] Categories { get; set; }
        public StudyPlanCourseDto[] Courses { get; set; }
        public int MajorId { get; set; }
    }
}
