using GradProjectServer.DTO.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Questions
{
    public class QuestionMetadataDto
    {
        public int Id { get; set; }
        public CourseDto Course { get; set; }
        public string Title { get; set; }
    }
}
