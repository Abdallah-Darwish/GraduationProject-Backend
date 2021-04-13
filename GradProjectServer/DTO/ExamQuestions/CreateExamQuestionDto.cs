using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.ExamQuestions
{
    //todo: validate exam is not approved or is admin
    //todo: validate question is approved
    public class CreateExamQuestionDto
    {
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int Order { get; set; }
    }
}
