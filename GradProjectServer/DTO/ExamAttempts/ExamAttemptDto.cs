using System;
using GradProjectServer.DTO.Exams;
using GradProjectServer.DTO.Users;

namespace GradProjectServer.DTO.ExamAttempts
{
    public class ExamAttemptDto
    {
        public int Id { get; set; }
        public ExamMetadataDto Exam { get; set; }
        public UserMetadataDto Owner { get; set; }
        public DateTimeOffset StartTime { get; set; }
    }
}