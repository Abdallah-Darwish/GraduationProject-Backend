using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure
{
    public class StudyPlanCoursePrerequisite
    {
        /// <summary>
        /// Start of edge
        /// </summary>
        public int PrerequisiteId { get; set; }
        /// <summary>
        /// End of edge
        /// </summary>
        public int CourseId { get; set; }
        public StudyPlanCourse Prerequisite { get; set; }
        public StudyPlanCourse Course { get; set; }
    }
}
