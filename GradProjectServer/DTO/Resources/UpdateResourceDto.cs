using System;
using GradProjectServer.Common;

namespace GradProjectServer.DTO.Resources
{
    public class UpdateResourceDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? FileName { get; set; }
        public string? FileBase64 { get; set; }
        public int? CreationYear { get; set; }
        public Semester? CreationSemester { get; set; }
        public bool? IsApproved { get; set; }
    }
}