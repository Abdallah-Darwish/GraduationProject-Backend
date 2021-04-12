using GradProjectServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    //todo: add update models
    public abstract class CreateSubQuestionDto
    {
        public string Content { get; set; }
        public SubQuestionType Type { get; set; }
        public int[]? Tags { get; set; }
        public int QuestionId { get; set; }
    }
}
