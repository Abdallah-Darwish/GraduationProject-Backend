using GradProjectServer.DTO.ExamSubQuestions;
using GradProjectServer.DTO.SubQuestions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.ExamQuestions
{
    public class ExamQuestionDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Title { get; set; }
        public int QuestionId { get; set; }
        public ExamSubQuestionDto[] ExamSubQuestions { get; set; }
    }
}
