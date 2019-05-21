// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Softeq.NetKit.Notifications.Web.Controllers
{
    [ProducesResponseType(typeof(List<ErrorDto>), 400)]
    [ProducesResponseType(typeof(List<ErrorDto>), 500)]
    //[Authorize]
    [Produces("application/json")]
    [Route("api/notifications")]
    [ApiVersion("1.0")]
    public class NotificationController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationController(ILogger<NotificationController> logger, INotificationService notificationService) : base(logger)
        {
            _notificationService = notificationService;
        }

        [ProducesResponseType(typeof(List<ErrorDto>), 404)]
        [ProducesResponseType(typeof(SendNotificationResponse), 200)]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> SendNotificationAsync([FromBody][Required] SendNotificationRequest request)
        {
            var result = await _notificationService.PostAsync(request);
            return Ok(result);
        }
    }
}
