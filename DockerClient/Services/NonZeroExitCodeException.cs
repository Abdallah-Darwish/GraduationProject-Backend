using System;

namespace DockerClient
{
    [Serializable]
    public class ContainerNonZeroExitCodeException : Exception
    {
        public int? ExitCode { get; set; }
        public string? StandardError { get; set; }
        public ContainerNonZeroExitCodeException()
        {
        }

        public ContainerNonZeroExitCodeException(int? exitCode = null, string? standardError = null)
        {
            ExitCode = exitCode;
            StandardError = standardError;
        }
        public ContainerNonZeroExitCodeException(string message) : base(message)
        {
        }
    }
}