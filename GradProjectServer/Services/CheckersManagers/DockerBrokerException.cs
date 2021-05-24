using System;
using DockerCommon;

namespace GradProjectServer.Services.CheckersManagers
{
    [Serializable]
    public class DockerBrokerException : Exception
    {
        public JobResult Result { get; }

        public DockerBrokerException(JobResult result)
        {
            Result = result;
        }
        public DockerBrokerException(JobResult result, string message) : base(message)
        {
            Result = result;
        }
    }
}