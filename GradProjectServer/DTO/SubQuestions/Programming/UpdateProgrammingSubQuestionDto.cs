using GradProjectServer.DTO.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace GradProjectServer.DTO.SubQuestions
{
    public class UpdateProgrammingSubQuestionDto : UpdateSubQuestionDto
    {
        public CreateProgramDto? Checker { get; set; }
    }
}
