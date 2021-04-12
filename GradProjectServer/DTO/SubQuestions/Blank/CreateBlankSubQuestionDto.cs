using GradProjectServer.DTO.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class CreateBlankSubQuestionDto : CreateSubQuestionDto
    {
        public CreateProgramDto? Checker { get; set; }
        public string? Answer { get; set; }
    }
}
