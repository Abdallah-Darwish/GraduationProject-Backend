using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System.Threading.Channels;
using DockerCommon;

namespace DockerBroker.Services
{
    public class JobDistributorService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly AppOptions _options;
        private PullSocket _serverPull;
        private PushSocket _serverPush;
        private Task _listeningTask;
        private CancellationTokenSource _listeningTaskCancellationToken;
        private ConcurrentDictionary<string, Job> _jobs;
        private AsyncAutoResetEvent _serverPushLock;
        public JobDistributorService(ILogger<JobDistributorService> logger, IHostApplicationLifetime appLifetime,
            IOptions<AppOptions> options)
        {
            _logger = logger;
            _options = options.Value;
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopped.Register(OnStopped);
        }

        public Task<JobResult> EnqueueBuildJob(string archivePath, string savePath)
        {
            BuildJob job = new()
            {
                RelativeArchivePath = archivePath,
                RelativeSavePath = savePath
            };
            return EnqueueJob(job);
        }
        public Task<JobResult> EnqueueCheckJob(string checkerDirectory, string resultDirectory, string submissionDirectory)
        {
            CheckJob job = new()
            {
                RelativeCheckerDirectory = checkerDirectory,
                RelativeResultDirectory = resultDirectory,
                RelativeSubmissionDirectory = submissionDirectory
            };
            return EnqueueJob(job);
        }
        public async Task<JobResult> EnqueueJob(Job job)
        {
           await _serverPushLock.WaitOne().ConfigureAwait(false);
           var msg = new NetMQMessage();
           if (job is BuildJob build)
           {
               msg.Append((int)JobType.Build);
               msg.Append(job.Id);
               msg.Append(build.RelativeArchivePath);
               msg.Append(build.RelativeSavePath);
           }
           else if(job is CheckJob check)
           {
               msg.Append((int)JobType.Check);
               msg.Append(job.Id);
               msg.Append(check.RelativeCheckerDirectory);
               msg.Append(check.RelativeResultDirectory);
               msg.Append(check.RelativeSubmissionDirectory);
           }
           _logger.LogInformation("Pushing Job(Id: {Id})", job.Id);
           _jobs.TryAdd(job.Id, job);
           _serverPush.SendMultipartMessage(msg);
           _serverPushLock.Release();
           return await job.CompletionTask.ConfigureAwait(false);
        }

        private async Task Listen(CancellationToken token)
        {
            await Task.Yield();
            while (!token.IsCancellationRequested)
            {
                var msg = _serverPull.ReceiveMultipartMessage();
                if (token.IsCancellationRequested)
                {
                    return;
                }

                string id = msg[0].ConvertToString(Encoding.Default);
                var result = (JobResult)msg[1].ConvertToInt32();
                if (!_jobs.TryRemove(id, out var job))
                {
                    _logger.LogWarning("Unrecognized Job (Id: {Id}) with (Result: {Result})", id, result);
                    return;
                }
                _logger.LogInformation("Job (Id: {Id}) is finished with (Result: {Result})", id, result);
                job.SetResult(result);
            }
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _jobs = new();
            _serverPushLock = new();
            _serverPull = new PullSocket(_options.ServerPullAddress);
            _serverPush = new PushSocket(_options.ServerPushAddress);
            _listeningTaskCancellationToken = new CancellationTokenSource();
            _listeningTask = Task.Factory.StartNew(() => Listen(_listeningTaskCancellationToken.Token), TaskCreationOptions.LongRunning).Unwrap();
            _logger.LogInformation("Created pull/push socket");
            _logger.LogInformation("Waiting for connections");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _listeningTaskCancellationToken.Cancel();
            _serverPull.Dispose();
            _serverPush.Dispose();
            _serverPushLock.Dispose();
            _jobs.Clear();
            _listeningTask = null;
            _listeningTaskCancellationToken = null;
            _serverPull = null;
            _serverPush = null;
            _serverPushLock = null;
            _jobs = null;
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            _logger.LogInformation("Started job distributor");
        }

        private void OnStopped()
        {
            _logger.LogInformation("Stopped job distributor");
        }
    }
}