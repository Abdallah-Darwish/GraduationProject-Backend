using System.IO;
using System.Threading.Tasks;

namespace GradProjectServer.Resources
{
    public static class ResourcesManager
    {
        private static readonly string AssemblyName = "GradProjectServer.Resources.";

        public static Stream GetStream(string name)
        {
            var assembly = typeof(ResourcesManager).Assembly;
            return assembly.GetManifestResourceStream(AssemblyName + name)!;
        }
        public static async Task<string> GetText(string name)
        {
            using var reader = new StreamReader(GetStream(name));
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }
    }
}
