using System.IO;
using Microsoft.Extensions.Options;

namespace GradProjectServer.Services
{
    public static class PathUtility
    {
        private static string _absolutePrefix;

        public static void Init(IOptions<AppOptions> options)
        {
            _absolutePrefix = options.Value.DataSaveDirectory;
            if (_absolutePrefix[^1] == Path.PathSeparator)
            {
                _absolutePrefix += Path.PathSeparator;
            }
        }

        public static string MakeRelative(string absolutePath) => absolutePath[_absolutePrefix.Length ..];
        public static string MakeAbsolute(string relativePath) => $"{_absolutePrefix}{relativePath}";
    }
}