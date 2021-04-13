using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.ExamSubQuestions
{
    public class CreateExamSubQuestionDto
    {
        public int ExamQuestionId { get; set; }
        public int SubQuestionId { get; set; }
        public float Weight { get; set; }
    }
}
