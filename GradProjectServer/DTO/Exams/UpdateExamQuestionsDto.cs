using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Exams
{
    public class UpdateExamQuestionsDto
    {
        public int ExamId { get; set; }
        public int[]? SubQuestionsToDelete { get; set; }
        public CreateExamSubQuestionDto[]? SubQuestionsToAdd { get; set; }
    }
}
