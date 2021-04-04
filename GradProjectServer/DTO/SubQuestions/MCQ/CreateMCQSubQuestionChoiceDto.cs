using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class CreateMCQSubQuestionChoiceDto
    {
        public string Content { get; set; }
        /// <summary>
        /// [-1, 1]
        /// </summary>
        public float Weight { get; set; }
    }
}
