using GradProjectServer.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.Exams.Entities
{
    public enum ExamType { TestBank, Quiz, University }
    public enum Semester { First, Second, Summer }
    public class Exam
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public ExamType Type { get; set; }
        public Semester Semester { get; set; }
        public string Name { get; set; }
        public ICollection<ExamSubQuestion> SubQuestions { get; set; }
        public TimeSpan Duration { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        //Course, volunteer, tags
    }
}
