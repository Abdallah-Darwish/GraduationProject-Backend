using System;
using System.Threading;
using System.Threading.Tasks;
using DockerCommon;

namespace DockerBroker.Services
{
    public abstract class Job
    {
        public string Id { get; }
        public JobResult? Result { get; private set; }
        private readonly TaskCompletionSource<JobResult> _completionSource = new();
        public Task<JobResult> CompletionTask => _completionSource.Task;
        
        public void SetResult(JobResult result)
        {
            Result = result;
            _completionSource.SetResult(result);
        }

        public Job()
        {
            Random rand = new();
            Id = $"{DateTime.Now.Ticks}|{rand.Next()}";
        }
    }
}