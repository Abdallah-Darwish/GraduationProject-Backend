using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.StudyPlanCourseCategories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.StudyPlans
{
    public class StudyPlanDto : StudyPlanMetadataDto
    {
        public CourseDto[] CoursesData { get; set; }
        public StudyPlanCourseCategoryDto[] Categories { get; set; }
        public StudyPlanCourseDto[] Courses { get; set; }
    }
}
