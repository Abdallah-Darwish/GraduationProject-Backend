using GradProjectServer.DTO.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions.Programming
{
    public class ProgrammingSubQuestion : SubQuestionDto
    {
        public string CheckerUrl { get; set; }
        public DependencyDto[] DependenciesIds { get; set; } 
    }
}
