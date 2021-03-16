using GradProjectServer.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public class ProgrammingSubQuestion : SubQuestion
    {
        public int CheckerId { get; set; }
        public Program Checker { get; set; }
    }
}
