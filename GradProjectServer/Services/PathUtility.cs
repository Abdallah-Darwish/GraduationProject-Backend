using System.IO;
using Microsoft.Extensions.Options;

namespace GradProjectServer.Services
{
    public class PathManager
    {
        private readonly string _absolutePrefix;

        public PathManager(IOptions<AppOptions> options)
        {
            _absolutePrefix = options.Value.DataSaveDirectory;
            if (_absolutePrefix[^1] == Path.PathSeparator)
            {
                _absolutePrefix += Path.PathSeparator;
            }
        }

        public string MakeRelative(string absolutePath) => absolutePath[_absolutePrefix.Length ..];
        public string MakeAbsolute(string relativePath) => $"{_absolutePrefix}{relativePath}";
    }
}