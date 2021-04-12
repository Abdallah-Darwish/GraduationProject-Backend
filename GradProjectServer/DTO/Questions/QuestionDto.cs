using GradProjectServer.DTO.Courses;
using GradProjectServer.DTO.SubQuestions;
using GradProjectServer.DTO.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Questions
{
    public class QuestionDto : QuestionMetadataDto
    {
        public string Content { get; set; }
        public SubQuestionMetadataDto[] SubQuestions { get; set; }
        public UserMetadataDto Volunteer { get; set; }
    }
}
