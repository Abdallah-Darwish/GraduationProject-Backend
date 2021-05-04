using GradProjectServer.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace GradProjectServer.Services.UserSystem
{
    public class NotLoggedInFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = UserManager.Instance.IdentifyUser(context.HttpContext.Request);

            if (user != null)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status403Forbidden,
                    Content = "CAN'T BE LOGGED IN",
                    ContentType = "text/plain"
                };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}