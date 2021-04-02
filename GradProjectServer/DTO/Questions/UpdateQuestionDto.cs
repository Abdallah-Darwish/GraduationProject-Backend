using GradProjectServer.DTO.SubQuestions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Questions
{
    //todo: validate all DTOs
    public class UpdateQuestionDto
    {
        public int QuestionId { get; set; }
        public string? Content { get; set; }
        public string? Title { get; set; }
        public int? CourseId { get; set; }
        public CreateSubQuestionDto[]? SubQuestionsToAdd { get; set; }
        public int[]? SubQuestionsToDelete { get; set; }
    }
}
