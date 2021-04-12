﻿using GradProjectServer.Common;
using GradProjectServer.DTO.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Exams
{
    /// <summary>
    /// Nulls mean it won't be updated
    /// </summary>
    public class UpdateExamDto
    {
        public int ExamId { get; set; }
        public int? Year { get; set; }
        public ExamType? Type { get; set; }
        public Semester? Semester { get; set; }
        public string? Name { get; set; }
        public TimeSpan? Duration { get; set; }
        public int? CourseId { get; set; }
        /// <summary>
        /// Will be considered only if the user is an admin.
        /// </summary>
        public bool? IsApproved { get; set; }
        public int[]? SubQuestionsToDelete { get; set; }
        public CreateExamSubQuestionDto[]? SubQuestionsToAdd { get; set; }

    }
}