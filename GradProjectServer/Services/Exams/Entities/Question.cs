using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsApproved { get; set; }
        public ICollection<SubQuestion> SubQuestions { get; set; }
        //Tags, Course, volunteer
    }
}
