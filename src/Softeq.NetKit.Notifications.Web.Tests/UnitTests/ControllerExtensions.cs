// Developed by Softeq Development Corporation
// http://www.softeq.com

using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Softeq.NetKit.Notifications.Web.Tests.UnitTests
{
    public static class ControllerExtensions
    {
        public static TController WithUser<TController>(this TController controller) where TController : Controller
        {
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, UnitTestBase.UserId),
                new Claim(JwtClaimTypes.Email, UnitTestBase.Email),
            }, "Test"));

            return controller;
        }
    }
}
