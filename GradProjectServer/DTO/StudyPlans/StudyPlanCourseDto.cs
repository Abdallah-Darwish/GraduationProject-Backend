using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.StudyPlans
{
    /// <summary>
    /// Its meant to embedded inside <see cref="StudyPlanDto"/>
    /// </summary>
    public class StudyPlanCourseDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int CategoryId { get; set; }
        public int[] PrerequisiteCourses { get; set; }
    }
}
