using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure
{
    public class StudyPlan
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int MajorId { get; set; }
        public Major Major { get; set; }
        public ICollection<CourseCategory> Categories { get; set; }
        public ICollection<StudyPlanCourse> Courses { get; set; }
    }
}
