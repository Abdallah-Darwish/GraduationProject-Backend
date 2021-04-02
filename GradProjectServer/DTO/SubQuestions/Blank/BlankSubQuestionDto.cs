using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class BlankSubQuestionDto : SubQuestionDto
    {
        public string? Answer { get; set; }
        public string? CheckerUrl { get; set; }
    }
}
