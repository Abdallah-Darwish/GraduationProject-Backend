using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.SubQuestions
{
    public class MCQSubQuestionChoiceDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Range [-1, 1]
        /// Where negative means its a wrong answer, it will be considered only if <see cref="MCQSubQuestionDto.IsCheckBox"/> is true.
        /// If <see cref="MCQSubQuestionDto.IsCheckBox"/> is false only one <see cref="Weight"/> can be > 0.
        /// </summary>
        public float Weight { get; set; }
    }
}
