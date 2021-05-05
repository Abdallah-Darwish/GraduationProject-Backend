using System;
using System.IO;
using System.Threading.Tasks;
using GradProjectServer.Services.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace GradProjectServer.Services.FilesManagers
{
    public class ResourceFileManager
    {
        public static string SaveDirectory { get; private set; }

        public static void Init(IServiceProvider sp)
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            SaveDirectory = Path.Combine(appOptions.DataSaveDirectory, "Resources");
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
        }

        public static string GetResourceFilePath(Resource res) =>
            Path.Combine(SaveDirectory, $"{res.Id}.{res.FileExtension}");

        public static string GetResourceFilePath(int resourceId, string extension) =>
            Path.Combine(SaveDirectory, $"{resourceId}.{extension}");

        public void DeleteResource(int resourceId, string extension)
        {
            var resourcePath = GetResourceFilePath(resourceId, extension);
            if (File.Exists(resourcePath))
            {
                File.Delete(resourcePath);
            }
        }


        public void DeleteResource(Resource resource) => DeleteResource(resource.Id, resource.FileExtension);

        /// <summary>
        /// Will update if it already exists.
        /// </summary>
        public async Task SaveResource(int resourceId, string extension, Stream content)
        {
            var resourcePath = GetResourceFilePath(resourceId, extension);
            await using var resourceFileStream = new FileStream(resourcePath, FileMode.OpenOrCreate,
                FileAccess.ReadWrite,
                FileShare.ReadWrite);
            await content.CopyToAsync(resourceFileStream).ConfigureAwait(false);
            resourceFileStream.SetLength(resourceFileStream.Position);
        }

        public Task SaveResource(Resource resource, Stream content) =>
            SaveResource(resource.Id, resource.FileExtension, content);

        public Stream GetResource(int resourceId, string extension)
        {
            var resourcePath = GetResourceFilePath(resourceId, extension);
            return new FileStream(resourcePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }

        public Stream GetResource(Resource resource) => GetResource(resource.Id, resource.FileExtension);
    }
}