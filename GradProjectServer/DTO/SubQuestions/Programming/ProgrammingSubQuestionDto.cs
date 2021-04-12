using GradProjectServer.DTO.Programs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions.Programming
{
    public class OwnedProgrammingSubQuestionDto : SubQuestionDto
    {
        public ProgramDto Checker { get; set; } 
    }
}
