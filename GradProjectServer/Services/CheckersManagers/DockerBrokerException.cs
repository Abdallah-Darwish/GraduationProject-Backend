using System;
using DockerCommon;

namespace GradProjectServer.Services.CheckersManagers
{
    [Serializable]
    public class DockerException : Exception
    {
        public DockerException(JobResult result) : base($"Docker didn't e")
        {
            
        }
    }
}