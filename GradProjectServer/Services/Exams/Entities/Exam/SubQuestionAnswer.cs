using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public class SubQuestionAnswer
    {
        public int AttemptId { get; set; }
        public int SubQuestionId { get; set; }
        public string? Answer { get; set; }
        public ExamAttempt Attempt { get; set; }
        public ExamSubQuestion SubQuestion { get; set; }
    }
}
