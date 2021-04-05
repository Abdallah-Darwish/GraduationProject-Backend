using GradProjectServer.Db;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.UserSystem
{
    public class LoggedInFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("MAH-TOKEN", out var token))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "NO_TOKEN",
                    ContentType = "text/plain"
                };
                return;
            }
            using var ctx = DbContextFac.Factory.CreateDbContext();

            var user = ctx.Users.FirstOrDefault(u => u.Token == token);
            if (user == null)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "INVALID_TOKEN",
                    ContentType = "text/plain"
                };
                return;
            }
            context.HttpContext.Features.Set(user);
            await next();
        }
    }
}
