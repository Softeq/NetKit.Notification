// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using Softeq.NetKit.Notifications.Web.Utility;

namespace Softeq.NetKit.Notifications.Web.Controllers
{
    [ProducesResponseType(typeof(List<ErrorDto>), 400)]
    [ProducesResponseType(typeof(List<ErrorDto>), 500)]
    [Authorize]
    [Produces("application/json")]
    [Route("api/notifications/history")]
    [ApiVersion("1.0")]
    public class NotificationHistoryController : BaseApiController
    {
        private readonly INotificationHistoryService _service;

        public NotificationHistoryController(ILogger<NotificationHistoryController> loggerFactory, INotificationHistoryService service) : base(loggerFactory)
        {
            _service = service;
        }

        [ProducesResponseType(typeof(NotificationSetResponse), 200)]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetNotificationsAsync([FromQuery] int pageSize = 50, [FromQuery]string startTime = null, [FromQuery]string endTime = null)
        {
            var userId = GetCurrentUserId();
            var options = FilterHelper.CreateOptions(startTime, endTime);

            var request = new GetNotificationsRequest(userId, options, pageSize);
            var res = await _service.ListAsync(request);
            return Ok(res);
        }

        [ProducesResponseType(typeof(NotificationResponse), 200)]
        [ProducesResponseType(typeof(List<ErrorDto>), 404)]
        [HttpGet]
        [Route("{notificationId}")]
        public async Task<IActionResult> GetNotificationByIdAsync(Guid notificationId)
        {
            var userId = GetCurrentUserId();
            var request = new GetNotificationRequest(userId, notificationId);
            var res = await _service.GetAsync(request);
            return Ok(res);
        }

        [ProducesResponseType(204)]
        [HttpDelete]
        [Route("")]
        public async Task<IActionResult> DeleteNotificationsAsync()
        {
            var userId = GetCurrentUserId();
            await _service.DeleteAllAsync(new UserRequest(userId));
            return NoContent();
        }
    }
}
