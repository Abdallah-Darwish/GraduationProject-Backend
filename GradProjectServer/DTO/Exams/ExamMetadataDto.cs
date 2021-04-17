using GradProjectServer.Common;
using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.Users;
using System;

namespace GradProjectServer.DTO.Exams
{
    public class ExamMetadataDto
    {
        public int Id { get; set; }
        public bool IsApproved { get; set; }
        public int Year { get; set; }
        public ExamType Type { get; set; }
        public Semester Semester { get; set; }
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public CourseDto Course { get; set; }
        public UserMetadataDto Volunteer { get; set; }
    }
}
