using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class CreateMCQSubQuestionDto : CreateSubQuestionDto
    {
        public bool IsCheckBox { get; set; }
        public CreateMCQSubQuestionChoiceDto[] Choices { get; set; }
    }
}
