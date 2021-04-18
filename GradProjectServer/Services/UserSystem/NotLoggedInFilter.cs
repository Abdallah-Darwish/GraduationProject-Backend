using GradProjectServer.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace GradProjectServer.Services.UserSystem
{
    public class NotLoggedInFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Cookies.TryGetValue(UserController.LoginCookieName, out var _))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "CAN'T HAVE A COOKIE",
                    ContentType = "text/plain"
                };
                return;
            }
            await next();
        }
    }
}
