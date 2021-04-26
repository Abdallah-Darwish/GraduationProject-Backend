using GradProjectServer.Common;

namespace GradProjectServer.DTO.Resources
{
    public class ResourceSearchFilterDto
    {
        public int[]? Courses { get; set; }
        public int? MinCreationYear { get; set; }
        public int? MaxCreationYear { get; set; }

        /// <summary>
        /// If user is Admin then he can get all not approved resources, otherwise only his not approved resources.
        /// </summary>
        public bool? IsApproved { get; set; }

        public Semester[]? CreationSemesters { get; set; }
        public string? NameMask { get; set; }
        public int[]? Volunteers { get; set; }
        public int Offset { get; set; }
        public int Count { get; set; }
    }
}