using System.Collections.Generic;

namespace GradProjectServer.DTO
{
    public class ErrorDTO
    {
        public string Description { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}