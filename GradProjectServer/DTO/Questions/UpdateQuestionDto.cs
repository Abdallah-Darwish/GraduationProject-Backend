﻿using GradProjectServer.DTO.SubQuestions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Questions
{
    public class UpdateQuestionDto
    {
        public int QuestionId { get; set; }
        public string? Content { get; set; }
        public string? Title { get; set; }
        public int? CourseId { get; set; }
    }
}