using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DockerClient
{
    public class DockerManager
    {
        public const string ImageName = "marje3sandbox";
        public static TimeSpan Timeout = TimeSpan.FromMinutes(5);
        public static string SaveDirectory { get; set; }

        public static void Init(IServiceProvider sp)
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            SaveDirectory = Path.Combine(appOptions.DataSaveDirectory, "DockerVolume");
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
        }

        private readonly ILogger<DockerManager> _logger;

        public DockerManager(ILogger<DockerManager> logger)
        {
            _logger = logger;
        }

        public async Task Build(string archivePath, string savePath)
        {
            var extractionPath = Path.Join(SaveDirectory, $"Src{DateTime.Now.Ticks}");
            try
            {
                await using FileStream archiveStream =
                    new(archivePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                using ZipArchive archive = new(archiveStream);
                archive.ExtractToDirectory(extractionPath);

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                ProcessStartInfo containerStartInfo = new()
                {
                    FileName = "docker",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                var containerArgs = new[]
                {
                    "run",
                    "--rm",
                    "-v",
                    $"{extractionPath}:/BuildSource:ro",
                    "-v",
                    $"{savePath}:/BuildOutput",
                    ImageName,
                    "build"
                };
                foreach (var arg in containerArgs)
                {
                    containerStartInfo.ArgumentList.Add(arg);
                }

                var container = Process.Start(containerStartInfo)!;

                using CancellationTokenSource containerWaitingCancellationToken = new();
                containerWaitingCancellationToken.CancelAfter(Timeout);
                await container.WaitForExitAsync(containerWaitingCancellationToken.Token).ConfigureAwait(false);
                if (!container.HasExited)
                {
                    container.Kill(true);
                    throw new TimeoutException("Container timed out.");
                }

                if (container.ExitCode != 0)
                {
                    var err = await container.StandardError.ReadToEndAsync().ConfigureAwait(false);
                    _logger.LogWarning(
                        "Failed to start a building container with {{ ArchivePath: {ArchivePath}, SavePath: {SavePath} }}, with error: {Error}",
                        archivePath, savePath, err);
                    throw new ContainerNonZeroExitCodeException();
                }
            }
            finally
            {
                if (Directory.Exists(extractionPath))
                {
                    Directory.Delete(extractionPath, true);
                }
            }
        }

        public async Task Check(string checkerDirectory, string resultDirectory, string submissionDirectory)
        {
            if (!Directory.Exists(resultDirectory))
            {
                Directory.CreateDirectory(resultDirectory);
            }

            ProcessStartInfo containerStartInfo = new()
            {
                FileName = "docker",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var containerArgs = new[]
            {
                "run",
                "--rm",
                "-v",
                $"{checkerDirectory}:/Checker:ro",
                "-v",
                $"{resultDirectory}:/Result",
                "-v",
                $"{submissionDirectory}:/Submission:ro",
                ImageName,
                "check"
            };
            foreach (var arg in containerArgs)
            {
                containerStartInfo.ArgumentList.Add(arg);
            }

            var container = Process.Start(containerStartInfo)!;

            using CancellationTokenSource containerWaitingCancellationToken = new();
            containerWaitingCancellationToken.CancelAfter(Timeout);
            await container.WaitForExitAsync(containerWaitingCancellationToken.Token).ConfigureAwait(false);
            if (!container.HasExited)
            {
                container.Kill(true);
                throw new TimeoutException("Container timed out.");
            }

            if (container.ExitCode != 0)
            {
                var err = await container.StandardError.ReadToEndAsync().ConfigureAwait(false);
                _logger.LogWarning(
                    "Failed to start a checking container with {{ CheckerDirectory: {CheckerDirectory}, ResultDirectory: {ResultDirectory}, SubmissionDirectory: {SubmissionDirectory} }}, with error: {Error}",
                    checkerDirectory, resultDirectory, submissionDirectory, err);

                throw new ContainerNonZeroExitCodeException();
            }
        }

        public async Task<bool> CheckIfImageIsBuilt()
        {
            ProcessStartInfo containerStartInfo = new()
            {
                FileName = "docker",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var containerArgs = new[] {"images"};
            foreach (var arg in containerArgs)
            {
                containerStartInfo.ArgumentList.Add(arg);
            }

            var docker = Process.Start(containerStartInfo)!;
            await docker.WaitForExitAsync().ConfigureAwait(false);
            if (docker.ExitCode != 0)
            {
                return false;
            }

            await docker.StandardOutput.ReadLineAsync().ConfigureAwait(false);
            string line;
            while (docker.StandardOutput.EndOfStream == false)
            {
                line = await docker.StandardOutput.ReadLineAsync().ConfigureAwait(false);
                if (line == null)
                {
                    break;
                }

                if (line.Split(new char[] {' ', '\t'},
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0] == ImageName)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task BuildImage()
        {
            var marje3SandBoxDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Marje3SandBoxDir");
            if (Directory.Exists(marje3SandBoxDirectory))
            {
                Directory.Delete(marje3SandBoxDirectory, true);
            }

            Directory.CreateDirectory(marje3SandBoxDirectory);
            File.Copy("DockerClient.py", Path.Join(marje3SandBoxDirectory, "DockerClient.py"));
            File.Copy("Marje3SandBox", Path.Join(marje3SandBoxDirectory, "Dockerfile"));
            ProcessStartInfo containerStartInfo = new()
            {
                FileName = "docker",
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            var containerArgs = new[]
            {
                "build",
                "-t",
                ImageName,
                "Marje3SandBoxDir"
            };
            foreach (var arg in containerArgs)
            {
                containerStartInfo.ArgumentList.Add(arg);
            }

            var builder = Process.Start(containerStartInfo)!;
            await builder.WaitForExitAsync().ConfigureAwait(false);
            if (builder.ExitCode != 0)
            {
                var err = await builder.StandardError.ReadToEndAsync().ConfigureAwait(false);
                _logger.LogWarning("Failed to build sandbox image with error: {Error}", err);

                throw new ContainerNonZeroExitCodeException();
            }
        }
    }
}