using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.ExamSubQuestions
{
    public class UpdateExamSubQuestionDto
    {
        public int Id { get; set; }
        public float? Weight { get; set; }
    }
}
