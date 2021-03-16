using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure
{
    public class Major
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<StudyPlan> StudyPlans { get; set; }
    }
}
