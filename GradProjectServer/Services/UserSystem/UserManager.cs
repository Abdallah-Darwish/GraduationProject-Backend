using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using GradProjectServer.Services.EntityFramework;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;

namespace GradProjectServer.Services.UserSystem
{
    public class UserManager
    {
        public static readonly string LoginCookieName = "marje3";
        public static readonly string LoginHeaderName = "Authorization";
        private string GenerateToken(User user) => $"{user.Id}:{DateTime.UtcNow.Ticks}";

        /// <summary>
        /// To be used by filters only.
        /// </summary>
        public static UserManager Instance { get; private set; }

        public static void InitInstance(IServiceProvider sp)
        {
            var fac = sp.GetRequiredService<IDbContextFactory<AppDbContext>>();
            Instance = new UserManager(fac.CreateDbContext());
        }

        /// <summary>
        /// Its public to be used only for seeding
        /// </summary>
        public static string HashPassword(string password) => password;

        private readonly AppDbContext _dbContext;

        public UserManager(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool ValidateImage(Stream? imageStream)
        {
            if (imageStream == null || imageStream.Length == 0)
            {
                return true;
            }

            using var image = SKImage.FromEncodedData(imageStream);
            return image != null;
        }

        public bool ValidateImage(string? base64ImageBytes)
        {
            if (string.IsNullOrEmpty(base64ImageBytes))
            {
                return true;
            }

            using var stream = new MemoryStream();
            stream.Write(Convert.FromBase64String(base64ImageBytes));
            stream.Position = 0;
            return ValidateImage(stream);
        }

        public Task UpdateImage(int userId, Stream? imageStream)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateImage(int userId, string? base64ImageBytes)
        {
            if (string.IsNullOrEmpty(base64ImageBytes))
            {
                return;
            }

            await using var stream = new MemoryStream();
            stream.Write(Convert.FromBase64String(base64ImageBytes));
            stream.Position = 0;
            await UpdateImage(userId, stream).ConfigureAwait(false);
        }

        public Task<Stream?> GetImage(int userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> Login(string email, string password, IResponseCookies cookies)
        {
            email = email.ToLowerInvariant();
            var user = _dbContext.Users.FirstOrDefault(u => u.Email.ToLower() == email);
            if (user == null || user.PasswordHash != HashPassword(password))
            {
                return null;
            }

            user.Token = GenerateToken(user);
            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                Expires = DateTimeOffset.MaxValue,
                IsEssential = true,
                HttpOnly = false,
                Secure = false,
            };
            cookies.Append(LoginCookieName, user.Token, cookieOptions);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return user;
        }

        public async Task Logout(User user, IResponseCookies cookies)
        {
            var myUser = await _dbContext.Users.FindAsync(user.Id).ConfigureAwait(false);
            myUser.Token = null;
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public User? GetUserByToken(string token)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Token != null && u.Token.ToLower() == token);
            return user;
        }

        public async Task UpdateUser(int userId, string? newPassword, string? newName, bool? newIsAdmin,
            int? newStudyPlanId)
        {
            var user = await _dbContext.Users.FindAsync(userId).ConfigureAwait(false);
            if (newPassword != null)
            {
                user.PasswordHash = HashPassword(newPassword);
            }

            if (newName != null)
            {
                user.Name = newName;
            }

            if (newIsAdmin != null)
            {
                user.IsAdmin = newIsAdmin.Value;
            }

            if (newStudyPlanId != null)
            {
                user.StudyPlanId = newStudyPlanId.Value;
            }

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<User> SignUp(string email, string password, string name, int studyPlanId)
        {
            User user = new()
            {
                Email = email,
                IsAdmin = false,
                Name = name,
                PasswordHash = HashPassword(password),
                StudyPlanId = studyPlanId,
                Token = null
            };
            await _dbContext.Users.AddAsync(user).ConfigureAwait(false);
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            return user;
        }

        public User? IdentifyUser(HttpRequest request)
        {
            request.Cookies.TryGetValue(LoginCookieName, out var cookie);
            request.Headers.TryGetValue(LoginHeaderName, out var headers);
            if (headers.Count > 1)
            {
                return null;
            }

            var header = headers.Count == 0 ? null : headers[0];
            string token;
            if (cookie == null && header == null)
            {
                return null;
            }

            if (cookie!=null && header!= null)
            {
                if (cookie[0] != header[0])
                {
                    return null;
                }

                token = cookie;
            }
            else if (cookie != null)
            {
                token = cookie;
            }
            else
            {
                token = header;
            }

            var user = _dbContext.Users.FirstOrDefault(u => u.Token == token);
            return user;
        }
        //Todo: Add methods to validate user when signing up, or we can keep them in their own validator
    }
}