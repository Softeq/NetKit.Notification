// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Notifications.Domain.Models.Errors;

namespace Softeq.NetKit.Notifications.Domain.Exceptions
{
    public interface IServiceException
    {
        List<ErrorDto> Errors { get; set; }
    }
}
