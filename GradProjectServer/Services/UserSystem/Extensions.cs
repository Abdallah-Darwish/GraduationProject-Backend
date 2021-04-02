using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GradProjectServer.Services.UserSystem
{
    public static class Extensions
    {
        public static User? GetUser(this ControllerBase controller) => GetUser(controller.HttpContext);
        public static User? GetUser(this HttpContext context) => context.Items[1234] as User;
    }
}
