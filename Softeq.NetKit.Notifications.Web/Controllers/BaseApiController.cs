// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Softeq.NetKit.Notifications.Web.Controllers
{
    public class BaseApiController : Controller
    {
        protected readonly ILogger Logger;

        public BaseApiController(ILogger logger)
        {
            Logger = logger;
        }

        #region helpers

        protected string GetCurrentUserId()
        {
            return this.User.FindFirstValue(JwtClaimTypes.Subject);
        }

        protected string GetCurrentUserEmail()
        {
            return this.User.FindFirstValue(JwtClaimTypes.Email);
        }

        #endregion
    }
}
