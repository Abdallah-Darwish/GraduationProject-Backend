using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.UserSystem
{
    public interface IUserProfilePictureRepository
    {
        Task Validate(Stream imageStream);
        Task Validate(string base64ImageBytes);
        Task Save(int userId, Stream imageStream);
        Task Save(int userId, string base64ImageBytes);
        Task<Stream> Get(int userId);
    }
}
