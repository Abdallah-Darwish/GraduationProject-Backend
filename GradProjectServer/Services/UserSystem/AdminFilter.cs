using GradProjectServer.Controllers;
using GradProjectServer.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.UserSystem
{
    public class AdminFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Cookies.TryGetValue(UserController.LoginCookieName, out var cookie))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Content = "NO COOKIE",
                    ContentType = "text/plain"
                };
                return;
            }
            using var ctx = DbContextFac.Factory.CreateDbContext();

            var user = ctx.Users.FirstOrDefault(u => u.Token == cookie);
            if (user == null)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Content = "INVALID COOKIE",
                    ContentType = "text/plain"
                };
                return;
            }
            if (!user.IsAdmin)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "NOT ADMIN",
                    ContentType = "text/plain"
                };
                return;
            }
            context.HttpContext.Features.Set(user);
            await next();
        }
    }
}
