using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Exams
{
    public class CreateExamSubQuestionDto
    {
        public int QuestionId { get; set; }
        public float Weight { get; set; }
    }
}
