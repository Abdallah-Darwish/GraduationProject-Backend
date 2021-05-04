using GradProjectServer.DTO.Courses;
using System;
using GradProjectServer.Common;
using GradProjectServer.DTO.Users;

namespace GradProjectServer.DTO.Resources
{
    public class ResourceDto
    {
        public int Id { get; set; }
        public CourseDto Course { get; set; }
        public int CreationYear { get; set; }
        public Semester CreationSemester { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Not prefixed with dot.
        /// </summary>
        public string FileExtension { get; set; }

        public bool IsApproved { get; set; }
        public UserMetadataDto Volunteer { get; set; }
        public ResourceType Type { get; set; }
    }
}