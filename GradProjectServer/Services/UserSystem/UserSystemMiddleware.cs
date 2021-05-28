using System.Globalization;
using System.Threading.Tasks;
using GradProjectServer.Services.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace GradProjectServer.Services.UserSystem
{
    public class UserSystemMiddleware 
    {
        private readonly RequestDelegate _next;

        public UserSystemMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserManager userManager)
        {
            var user = await userManager.IdentifyUser(context.Request).ConfigureAwait(false);
            if (user != null)
            {
                 context.Features.Set(user);
            }
            await _next(context);
        }
    }

    public static class UserSystemMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserSystem(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserSystemMiddleware>();
        }
    }
}