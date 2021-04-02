using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Programs
{
    public class CreateProgramDto
    {
        public string ArchiveBase64 { get; set; }
        public int[] DependenciesIds { get; set; }
    }
}
