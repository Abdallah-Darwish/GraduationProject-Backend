using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GradProjectServer.Services
{
    public static class PathUtility
    {
        private static string _absolutePrefix;

        public static void Init(IServiceProvider sp)
        {
            var options = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            _absolutePrefix = options.DataSaveDirectory;
            if (_absolutePrefix[^1] == Path.PathSeparator)
            {
                _absolutePrefix += Path.PathSeparator;
            }
        }

        public static string MakeRelative(string absolutePath) => absolutePath[_absolutePrefix.Length ..];
        public static string MakeAbsolute(string relativePath) => $"{_absolutePrefix}{relativePath}";
    }
}