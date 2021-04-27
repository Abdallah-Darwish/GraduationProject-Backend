using System;
using GradProjectServer.Common;

namespace GradProjectServer.DTO.Resources
{
    public class UpdateResourceDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        /// <summary>
        /// Don't prefix with dot.
        /// </summary>
        public string? FileExtension { get; set; }
        public string? FileBase64 { get; set; }
        public int? CreationYear { get; set; }
        public Semester? CreationSemester { get; set; }
        /// <summary>
        /// Must be null if user is not an admin.
        /// </summary>
        public bool? IsApproved { get; set; }
        
    }
}