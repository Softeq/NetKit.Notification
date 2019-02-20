// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.Serilog.Extension;

namespace Softeq.NetKit.Notifications.Web.Infrastructure.ErrorHandling
{
    internal class GlobalExceptionFilter : ExceptionFilterAttribute
    {
        private readonly ILogger _logger;

        public GlobalExceptionFilter(ILogger logger)
        {
            _logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            var username = context.HttpContext.User.FindFirstValue(JwtClaimTypes.Name);
            var url = context.HttpContext.Request.Path.Value;
            if (context.Exception is ServiceException exception)
            {
                _logger.Event(PropertyNames.EventId).With.Message("ServiceException: Requester UserName: {username}, Url: {url}", username, url).Exception(exception).AsError();

                var response = new ObjectResult(exception.Errors) { StatusCode = exception.ToHttpStatusCode() };
                context.Result = response;
            }
            else
            {
                _logger.Event(PropertyNames.EventId).With.Message("Exception: UserName: {username}, Url: {url}", username, url).Exception(context.Exception).AsError();
                var response = new ObjectResult(new List<ErrorDto>
                    {
                        new ErrorDto(ErrorCode.UnknownError, "Something went wrong during your request.")
                    })
                { StatusCode = 500 };
                context.Result = response;
            }
        }
    }
}
