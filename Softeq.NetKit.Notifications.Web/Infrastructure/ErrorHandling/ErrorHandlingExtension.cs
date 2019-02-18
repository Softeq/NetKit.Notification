// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.Errors;

namespace Softeq.NetKit.Notifications.Web.Infrastructure.ErrorHandling
{
    internal static class ErrorHandlingExceptions
    {
        public static int ToHttpStatusCode(this ServiceException exception)
        {
            switch (exception)
            {
                case ConflictException conflictException:
                    return StatusCodes.Status409Conflict;
                case NotFoundException notFoundException:
                    return StatusCodes.Status404NotFound;
                case ValidationException validationException:
                    return StatusCodes.Status400BadRequest;
                default:
                    return StatusCodes.Status400BadRequest;
            }
        }

        public static List<ErrorDto> ToErrorModel(this ModelStateDictionary modelSate)
        {
            var modelError = new List<ErrorDto>();
            foreach (var error in modelSate)
            {
                modelError.Add(new ErrorDto
                {
                    Code = error.Key,
                    Description = error.Value.Errors.FirstOrDefault()?.ErrorMessage

                });
            }
            return modelError;
        }
    }
}
