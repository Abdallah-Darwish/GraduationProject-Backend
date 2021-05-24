using System;

namespace GradProjectServer.Services.FilesManagers.Temp
{
    public class TempDirectory : IDisposable
    {
        public string Directory { get; init; }
        public string RelativeDirectory => PathUtility.MakeRelative(Directory);
        public override string ToString() => Directory;

        private void ReleaseUnmanagedResources()
        {
            if (System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.Delete(Directory,true);
            }
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~TempDirectory()
        {
            ReleaseUnmanagedResources();
        }
    }
}