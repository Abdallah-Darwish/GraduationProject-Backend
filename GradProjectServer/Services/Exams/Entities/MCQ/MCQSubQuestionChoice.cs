using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public class MCQSubQuestionChoice
    {
        public byte Id { get; set; }
        public int QuestionId { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// Range [-1, 1]
        /// Where negative means its a wrong answer, it will be considered only if <see cref="MCQSubQuestion.IsCheckBox"/> is true.
        /// If <see cref="MCQSubQuestion.IsCheckBox"/> is false only one <see cref="Weight"/> can be > 0.
        /// </summary>
        public float Weight { get; set; }
        public MCQSubQuestion Question { get; set; }
    }
}
