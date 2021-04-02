using GradProjectServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Exams
{
    public class ExamSearchFilterDto
    {
        public int[]? Ids { get; set; }
        public string? NameMask { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public ExamType[]? Types { get; set; }
        public Semester[]? Semesters { get; set; }
        public TimeSpan? MinDuration { get; set; }
        public TimeSpan? MaxDuration { get; set; }
        public int[]? Courses { get; set; }
        public int[]? Tags { get; set; }
        public bool? IsApproved { get; set; }
        //useful for admins and user page
        /// <summary>
        /// Will be applied only if the user is an admin.
        /// </summary>
        public int[]? VolunteersIds { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}
