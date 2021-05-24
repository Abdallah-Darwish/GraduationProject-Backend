using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DockerCommon;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetMQ;
using NetMQ.Sockets;

namespace DockerClient
{
    public class CommandExecutorService : IHostedService
    {
        private readonly ILogger _logger;
        private readonly DockerManager _dockerManager;
        private readonly AppOptions _options;
        private PushSocket _serverPush;
        private PullSocket _serverPull;
        private NetMQRuntime _mqRuntime;
        private Task _listeningTask;
        private CancellationTokenSource _listeningTaskCancellationToken;

        public CommandExecutorService(ILogger<CommandExecutorService> logger, IHostApplicationLifetime appLifetime,
            DockerManager dockerManager, IOptions<AppOptions> options)
        {
            _dockerManager = dockerManager;
            _logger = logger;
            _options = options.Value;
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopped.Register(OnStopped);
        }

        private async Task HandleBuildMessage(NetMQMessage msg)
        {
            string jobId = msg[1].ConvertToString(Encoding.Default);
            string relativeArchivePath = msg[2].ConvertToString(Encoding.Default);
            string relativeSavePath = msg[3].ConvertToString(Encoding.Default);
            _logger.LogInformation("Starting new build job with id: {JobId}", jobId);
            _logger.LogDebug("Relative archive path: {RelativeArchivePath}", relativeArchivePath);
            _logger.LogDebug("Relative save path: {RelativeSavePath}", relativeSavePath);
            JobResult result = JobResult.Done;
            try
            {
                await _dockerManager.Build(relativeArchivePath, relativeSavePath).ConfigureAwait(false);
            }
            catch (TimeoutException ex)
            {
                result = JobResult.Tle;
            }
            catch (ContainerNonZeroExitCodeException)
            {
                result = JobResult.RuntimeError;
            }

            _logger.LogInformation("Job result: {Result}", result);
            var resultMessage = new NetMQMessage();
            resultMessage.Append(jobId, Encoding.Default);
            resultMessage.Append((int) result);
            _serverPush.SendMultipartMessage(resultMessage);
        }

        private async Task HandleCheckMessage(NetMQMessage msg)
        {
            string jobId = msg[1].ConvertToString(Encoding.Default);
            string relativeCheckerPath = msg[2].ConvertToString(Encoding.Default);
            string relativeResultPath = msg[3].ConvertToString(Encoding.Default);
            string relativeSubmissionPath = msg[4].ConvertToString(Encoding.Default);
            _logger.LogInformation("Starting new check job with id: {JobId}", jobId);
            _logger.LogDebug("Relative checker path: {RelativeCheckerPath}", relativeCheckerPath);
            _logger.LogDebug("Relative result path: {RelativeResultPath}", relativeResultPath);
            _logger.LogDebug("Relative submission path: {RelativeSubmissionPath}", relativeSubmissionPath);
            JobResult result = JobResult.Done;
            try
            {
                await _dockerManager.Check(relativeCheckerPath, relativeResultPath, relativeSubmissionPath).ConfigureAwait(false);
            }
            catch (TimeoutException ex)
            {
                result = JobResult.Tle;
            }
            catch (ContainerNonZeroExitCodeException)
            {
                result = JobResult.RuntimeError;
            }

            _logger.LogInformation("Job result: {Result}", result);
            var resultMessage = new NetMQMessage();
            resultMessage.Append(jobId, Encoding.Default);
            resultMessage.Append((int) result);
            _serverPush.SendMultipartMessage(resultMessage);
        }

        private async Task Listen(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var msg = _serverPull.ReceiveMultipartMessage();
                if (token.IsCancellationRequested)
                {
                    return;
                }

                var code = (JobType) msg[0].ConvertToInt32();
                if (code == JobType.Build)
                {
                    await HandleBuildMessage(msg);
                }
                else if (code == JobType.Check)
                {
                    await HandleCheckMessage(msg);
                }
                else
                {
                    _logger.LogWarning("Received unexpected message");
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
            {
                _mqRuntime = new();
                _serverPull = new PullSocket(_options.ClientPullAddress);
                _serverPush = new PushSocket(_options.ClientPushAddress);
                _listeningTaskCancellationToken = new CancellationTokenSource();
                _logger.LogInformation("Created pull/push sockets");
                _logger.LogInformation("Waiting for connections");
                _listeningTask = Task.Factory
                    .StartNew(() => Listen(_listeningTaskCancellationToken.Token), TaskCreationOptions.None).Unwrap();
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                _listeningTaskCancellationToken.Cancel();
                _mqRuntime.Dispose();
                _serverPull.Dispose();
                _serverPush.Dispose();
                _listeningTask = null;
                _listeningTaskCancellationToken = null;
                _serverPull = null;
                _serverPush = null;
                return Task.CompletedTask;
            }

            private void OnStarted()
            {
                _logger.LogInformation("Started command executor");
            }

            private void OnStopped()
            {
                _logger.LogInformation("Stopped command executor");
            }
        }
    }