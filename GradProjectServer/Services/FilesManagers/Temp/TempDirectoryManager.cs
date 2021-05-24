using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GradProjectServer.Services.FilesManagers.Temp
{
    /// <summary>
    /// Used to create temporary directories, mostly to be used in communication with checkers.
    /// It will delete all directories created by it on disposing.
    /// </summary>
    public class TempDirectoryManager : IDisposable
    {
        public static string SaveDirectory { get; private set; }
        private static readonly string MachineId;

        static TempDirectoryManager()
        {
            var invalidChars = Path.GetInvalidPathChars().ToImmutableHashSet();
            MachineId = string.Concat(Environment.MachineName.Select(c => invalidChars.Contains(c) ? 'X' : c));
        }
        public static void Init(IServiceProvider sp)
        {
            var options = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            SaveDirectory = Path.Combine(options.DataSaveDirectory, "Temp");
        }
        private readonly List<TempDirectory> _directories = new();

        public TempDirectory Create(string? nameHint = null)
        {
            nameHint = string.IsNullOrWhiteSpace(nameHint) ? "" : $"_{nameHint}";
            //Lets just hope it doesn't exist.
            string dir = Path.Combine(SaveDirectory,
                $"{MachineId}_{Thread.CurrentThread.ManagedThreadId}_{DateTime.Now.Ticks}{nameHint}");
            Directory.CreateDirectory(dir);
            TempDirectory result = new() {Directory = dir};
            
            _directories.Add(result);
            return result;
        }

        private void ReleaseUnmanagedResources()
        {
            foreach (var dir in _directories)
            {
                dir.Dispose();
            }
            _directories.Clear();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~TempDirectoryManager()
        {
            ReleaseUnmanagedResources();
        }
    }
}