using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure
{
    public class StudyPlanCourse
    {
        public int CourseId { get; set; }
        public int CategoryId { get; set; }
        public Course Course { get; set; }
        public CourseCategory Category { get; set; }
        public ICollection<StudyPlanCoursePrerequisite> Prerequisites { get; set; }
    }
}
