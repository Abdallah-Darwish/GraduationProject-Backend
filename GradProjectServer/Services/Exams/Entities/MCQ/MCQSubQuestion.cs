using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public class MCQSubQuestion : SubQuestion
    {
        public bool IsCheckBox { get; set; }
        public ICollection<MCQSubQuestionChoice> Choices { get; set; }
    }
}
