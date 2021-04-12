using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Programs
{
    public class ProgramDto
    {
        public int Id { get; set; }
        public DependencyDto[] Dependencies { get; set; }
    }
}
