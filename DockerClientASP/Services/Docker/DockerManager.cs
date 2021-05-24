using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DockerClientASP.Services.Dockerx
{
    public class DockerManager
    {
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
                    UseShellExecute = true,
                    FileName = "docker",
                };
                var containerArgs = new string[]
                {
                    "run",
                    "-v",
                    $"{extractionPath}:/BuildSource:ro",
                    "-v",
                    $"{savePath}:/BuildOutput",
                    "Marje3SandBox",
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
                    throw new Exception("Container timed out.");
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

        public async Task Check(string checkerPath, string resultDirectory)
        {
            if (!Directory.Exists(resultDirectory))
            {
                Directory.CreateDirectory(resultDirectory);
            }

            ProcessStartInfo containerStartInfo = new()
            {
                UseShellExecute = true,
                FileName = "docker",
            };
            var containerArgs = new string[]
            {
                "run",
                "-v",
                $"{checkerPath}:/Checker:ro",
                "-v",
                $"{resultDirectory}:/Result",
                "Marje3SandBox",
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
                throw new Exception("Container timed out.");
            }
        }
    }
}