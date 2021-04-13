using GradProjectServer.DTO.SubQuestions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Exams
{
    public class ExamSubQuestionDto
    {
        public SubQuestionMetadataDto SubQuestion { get; set; }
        public float Weight { get; set; }
    }
}
