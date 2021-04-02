using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.SubQuestions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Questions
{
    public class QuestionDto : QuestionMetadataDto
    {
        public string Content { get; set; }
        public SubQuestionMetadataDto[] Questions { get; set; }
    }
}
