using GradProjectServer.Services.Exams.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Infrastructure
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CreditHours { get; set; }
        public ICollection<Exam> Exams { get; set; }
    }
}
