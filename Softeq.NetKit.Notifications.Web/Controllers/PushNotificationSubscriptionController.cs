// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.PushNotification.Request;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;

namespace Softeq.NetKit.Notifications.Web.Controllers
{
    [ProducesResponseType(typeof(List<ErrorDto>), 500)]
    [ProducesResponseType(typeof(List<ErrorDto>), 400)]
    [Authorize]
    [Produces("application/json")]
    [Route("api/notifications/push/subscription")]
    [ApiVersion("1.0")]
    public class PushNotificationSubscriptionController : BaseApiController
    {
        private readonly IPushNotificationSubscriptionService _service;

        public PushNotificationSubscriptionController(ILogger<PushNotificationSubscriptionController> logger, IPushNotificationSubscriptionService service) : base(logger)
        {
            _service = service;
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        [Route("")]
        public async Task<IActionResult> UnsubscribeDeviceAsync([FromBody][Required]PushDeviceRequest request)
        {
            request.UserId = GetCurrentUserId();
            await _service.UnsubscribeDeviceAsync(request);
            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(204)]
        [Route("me")]
        public async Task<IActionResult> UnsubscribeUserAsync()
        {
            var userId = GetCurrentUserId();
            await _service.UnsubscribeUserAsync(new UserRequest(userId));
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [Route("")]
        public async Task<IActionResult> SubscribeDeviceAsync([FromBody][Required]PushDeviceRequest request)
        {
            request.UserId = GetCurrentUserId();
            await _service.CreateOrUpdateSubscriptionAsync(request);
            return Ok();
        }
    }
}
