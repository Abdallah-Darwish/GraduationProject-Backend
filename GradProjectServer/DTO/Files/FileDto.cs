using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO
{
    public class FileDto
    {
        public string Name { get; set; }
        /// <summary>
        /// In bytes.
        /// </summary>
        public long Size { get; set; }
        public string DownloadUrl { get; set; }
    }
}
