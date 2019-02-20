// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Softeq.NetKit.Notifications.Web.Controllers
{
    [ProducesResponseType(typeof(List<ErrorDto>), 404)]
    [ProducesResponseType(typeof(List<ErrorDto>), 400)]
    [ProducesResponseType(typeof(List<ErrorDto>), 500)]
    [Authorize]
    [Produces("application/json")]
    [Route("api/settings")]
    [ApiVersion("1.0")]
    public class SettingsController : BaseApiController
    {
        private readonly ISettingsService _service;

        public SettingsController(ILogger<SettingsController> logger, ISettingsService service) : base(logger)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(NotificationSettingsResponse), 200)]
        [Route("notifications")]
        public async Task<IActionResult> GetNotificationSettingsAsync()
        {
            var userId = GetCurrentUserId();
            var res = await _service.GetNotificationSettingsAsync(new UserRequest(userId));
            return Ok(res);
        }

        [HttpPut]
        [ProducesResponseType(typeof(NotificationSettingsResponse), 200)]
        [Route("notifications")]
        public async Task<IActionResult> UpdateNotificationSettingsAsync([Required][FromBody]UpdateNotificationSettingsRequest request)
        {
            request.UserId = GetCurrentUserId();
            var res = await _service.UpdateNotificationSettingsAsync(request);
            return Ok(res);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserSettingsResponse), 201)]
        [ProducesResponseType(typeof(List<ErrorDto>), 409)]
        [Route("", Name = "CreateSettings")]
        public async Task<IActionResult> CreateUserSettingsAsync([FromBody][Required] UserProfileRequest request)
        {
            request = request ?? new UserProfileRequest();
            request.UserId = GetCurrentUserId();
            var result = await _service.CreateUserSettingsAsync(request);
            return CreatedAtRoute("GetSettings", result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserProfileResponse), 200)]
        [ProducesResponseType(typeof(List<ErrorDto>), 404)]
        [Route("", Name = "GetSettings")]
        public async Task<IActionResult> GetUserSettingsAsync()
        {
            var userId = GetCurrentUserId();
            var result = await _service.GetUserSettingsAsync(new UserRequest(userId));
            return Ok(result);
        }

        [HttpPut]
        [ProducesResponseType(typeof(UserProfileResponse), 200)]
        [ProducesResponseType(typeof(List<ErrorDto>), 404)]
        [Route("")]
        public async Task<IActionResult> UpdateUserSettingsAsync([FromBody][Required] UserProfileRequest request)
        {
            request.UserId = GetCurrentUserId();
            var result = await _service.UpdateUserSettingsAsync(request);
            return Ok(result);
        }
    }
}