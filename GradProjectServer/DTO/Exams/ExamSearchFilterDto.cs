using GradProjectServer.Common;
using System;

namespace GradProjectServer.DTO.Exams
{
    public class ExamSearchFilterDto
    {
        public string? NameMask { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public ExamType[]? Types { get; set; }
        public Semester[]? Semesters { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public int[]? Courses { get; set; }
        public int[]? Tags { get; set; }

        public bool? IsApproved { get; set; }

        //useful for admins and user page
        /// <summary>
        /// Will be applied only if the user is an admin,
        /// else if its not null or empty then only the user id will be considered.
        /// </summary>
        public int[]? VolunteersIds { get; set; }

        public int Offset { get; set; }
        public int Count { get; set; }
        public bool Metadata { get; set; }
    }
}