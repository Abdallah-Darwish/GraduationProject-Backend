using GradProjectServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Exams
{
    public class CreateExamDto
    {
        public int Year { get; set; }
        public ExamType Type { get; set; }
        public Semester Semester { get; set; }
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public int CourseId { get; set; }
    }
}
