using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class OwnedMCQSubQuestionDto : MCQSubQuestionDto
    {
        public OwnedMCQSubQuestionChoiceDto[] Choices { get; set; }
    }
}
