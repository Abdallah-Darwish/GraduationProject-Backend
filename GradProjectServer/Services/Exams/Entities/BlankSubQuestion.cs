using GradProjectServer.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    /// <summary>
    /// If both <see cref="Checker"/> and <see cref="Answer"/> exist then <see cref="Checker"/> will be used and <see cref="Answer"/> will be supplied to it.
    /// </summary>
    public class BlankSubQuestion : SubQuestion
    {
        public int? CheckerId { get; set; }
        public Program? Checker { get; set; }
        public string? Answer { get; set; }
    }
}
