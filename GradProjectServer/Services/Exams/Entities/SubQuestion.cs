using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public enum SubQuestionType { MultipleChoice, Blank, Code }
    public class SubQuestion
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public SubQuestionType Type { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        public ICollection<SubQuestionTag> Tags { get; set; }
    }
}
