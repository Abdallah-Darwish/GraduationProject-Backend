using GradProjectServer.Common;
using System;

namespace GradProjectServer.DTO.Exams
{
    public class CreateExamDto
    {
        public int Year { get; set; }
        public ExamType Type { get; set; }
        public Semester Semester { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// In milliseconds.
        /// </summary>
        public int Duration { get; set; }
        public int CourseId { get; set; }
    }
}
