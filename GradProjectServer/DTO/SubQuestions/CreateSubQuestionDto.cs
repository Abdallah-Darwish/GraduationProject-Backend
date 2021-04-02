using GradProjectServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class CreateSubQuestionDto
    {
        public string Content { get; set; }
        public SubQuestionType Type { get; set; }
        public int[] TagsIds { get; set; }
    }
}
