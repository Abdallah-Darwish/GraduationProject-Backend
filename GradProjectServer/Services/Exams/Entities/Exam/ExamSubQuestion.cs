using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public class ExamSubQuestion
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int SubQuestionId { get; set; }
        public float Weight { get; set; }
        public Exam Exam { get; set; }
        public SubQuestion SubQuestion { get; set; }
    }
}
