using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure
{
    public class CourseCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AllowedCreditHours { get; set; }
        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }
        public ICollection<StudyPlanCourse> Courses { get; set; } 
    }
}
