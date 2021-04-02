using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.DTO
{
    public class ErrorDTO
    {
        public string Description { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
