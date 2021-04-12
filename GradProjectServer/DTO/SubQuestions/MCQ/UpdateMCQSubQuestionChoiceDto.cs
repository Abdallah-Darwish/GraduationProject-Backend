using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class UpdateMCQSubQuestionChoiceDto
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public float? Weight { get; set; }
    }
}
