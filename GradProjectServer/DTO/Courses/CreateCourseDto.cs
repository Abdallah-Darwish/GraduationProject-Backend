using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Courses
{
    public class CreateCourseDto
    {
        public string Name { get; set; }
        public int CreditHours { get; set; }
    }
}
