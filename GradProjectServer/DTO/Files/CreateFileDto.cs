using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO.Files
{
    public class CreateFileDto
    {
        public string Name { get; set; }
        /// <summary>
        /// Base64 encoding of the file bytes.
        /// </summary>
        public string Content { get; set; }
    }
}
