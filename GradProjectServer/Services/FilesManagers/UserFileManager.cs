using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using GradProjectServer.Services.UserSystem;
using SkiaSharp;

namespace GradProjectServer.Services.FilesManagers
{
    public class UserFileManager
    {
        public static string SaveDirectory { get; private set; }

        public static void Init(IServiceProvider sp)
        {
            var appOptions = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            SaveDirectory = Path.Combine(appOptions.DataSaveDirectory, "UsersProfilePictures");
            if (!Directory.Exists(SaveDirectory))
            {
                Directory.CreateDirectory(SaveDirectory);
            }
        }

        /// <summary>
        /// Its public to be used only by <see cref="User.Seed"/>.
        /// </summary>
        public static string GetProfilePicturePath(int userId) => Path.Combine(SaveDirectory, $"{userId}.jpg");

        public static string GetProfilePicturePath(User user) => GetProfilePicturePath(user.Id);

        public async Task SaveProfilePicture(int userId, Stream picture)
        {
            var picPath = GetProfilePicturePath(userId);

            await using var fileStream =
                new FileStream(picPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            using var pic = SKImage.FromEncodedData(picture);
            using var encodedPic = pic.Encode(SKEncodedImageFormat.Jpeg, 100);
            encodedPic.SaveTo(fileStream);
            fileStream.SetLength(fileStream.Position);
            await fileStream.FlushAsync().ConfigureAwait(false);
        }

        public Task SaveProfilePicture(User user, Stream picture) => SaveProfilePicture(user.Id, picture);

        public Stream? GetProfilePicture(int userId)
        {
            var picPath = GetProfilePicturePath(userId);
            return !File.Exists(picPath)
                ? null
                : new FileStream(picPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        }


        public Stream? GetProfilePicture(User user) => GetProfilePicture(user.Id);

        public bool ValidateProfilePicture(Stream picture)
        {
            using var image = SKImage.FromEncodedData(picture);
            return image != null;
        }

        public void DeleteProfilePicture(int userId)
        {
            var picPath = GetProfilePicturePath(userId);
            if (File.Exists(picPath))
            {
                File.Delete(picPath);
            }
        }

        public void DeleteProfilePicture(User user) => DeleteProfilePicture(user.Id);
    }
}