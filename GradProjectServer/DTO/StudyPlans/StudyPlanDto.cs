using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.Majors;
using GradProjectServer.DTO.StudyPlanCourseCategories;

namespace GradProjectServer.DTO.StudyPlans
{
    public class StudyPlanDto : StudyPlanMetadataDto
    {
        public StudyPlanCourseCategoryDto[] CourseCategories { get; set; }
    }
}