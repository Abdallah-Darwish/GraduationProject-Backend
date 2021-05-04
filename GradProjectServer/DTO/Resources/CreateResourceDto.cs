using System;
using GradProjectServer.Common;
using GradProjectServer.DTO.Programs;

namespace GradProjectServer.DTO.Resources
{
    public class CreateResourceDto
    {
        public int CourseId { get; set; }
        public int CreationYear { get; set; }
        public Semester CreationSemester { get; set; }
        public string Name { get; set; }
        public CreateFileDto Resource { get; set; }

        public ResourceType Type { get; set; }
    }
}