using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Courses
{
    public class CourseSearchFilterDto
    {
        public string? NameMask { get; set; }
        public int? MinCreditHours { get; set; }
        public int? MaxCreditHours { get; set; }
        public bool? HasExams { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }

    }
}
