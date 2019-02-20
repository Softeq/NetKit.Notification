// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Softeq.NetKit.Notifications.Domain.Models.Localization;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using Softeq.NetKit.Notifications.Web.Controllers;
using Xunit;

namespace Softeq.NetKit.Notifications.Web.Tests.UnitTests
{
    public class SettingsControllerTests : UnitTestBase
    {
        private readonly ILogger<SettingsController> _logger = new Mock<ILogger<SettingsController>>().Object;

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetNotificationSettingsTest()
        {
            var service = new Mock<ISettingsService>();
            var response = new NotificationSettingsResponse
            {
                UserId = UserId
            };
            service.Setup(x => x.GetNotificationSettingsAsync(It.Is<UserRequest>(request => request.UserId == UserId)))
                .ReturnsAsync(response)
                .Verifiable();

            var controller = new SettingsController(_logger, service.Object).WithUser();
            var result = await controller.GetNotificationSettingsAsync();

            service.Verify();

            var res = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, res.Value);
            Assert.IsType<NotificationSettingsResponse>(res.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateNotificationSettingsTest()
        {
            var service = new Mock<ISettingsService>();
            var request = new UpdateNotificationSettingsRequest
            {
                Settings = new List<NotificationSettingModel>
                {
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Push,
                        Event = NotificationEvent.ArticleCreated,
                        Enabled = true
                    }
                }
            };

            var response = new NotificationSettingsResponse
            {
                UserId = UserId,
                Settings = new List<NotificationSettingModel>
                {
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Push,
                        Event = NotificationEvent.ArticleCreated,
                        Enabled = true
                    }
                }
            };

            UpdateNotificationSettingsRequest req = null;
            service.Setup(x => x.UpdateNotificationSettingsAsync(It.Is<UpdateNotificationSettingsRequest>(settingsRequest => settingsRequest == request && settingsRequest.UserId == UserId)))
                .Callback<UpdateNotificationSettingsRequest>(r => req = r)
                .ReturnsAsync(response)
                .Verifiable();

            var controller = new SettingsController(_logger, service.Object).WithUser();
            var result = await controller.UpdateNotificationSettingsAsync(request);

            service.Verify();
            Assert.NotNull(req);
            Assert.Equal(UserId, req.UserId);

            var res = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, res.Value);
            Assert.IsType<NotificationSettingsResponse>(res.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task CreateUserSettingsTest()
        {
            var service = new Mock<ISettingsService>();
            var request = new UserProfileRequest
            {
                Email = "alex@",
                FirstName = "alex",
                Language = LanguageName.Fr,
                LastName = "softeq",
                PhoneNumber = "123"
            };

            var response = new UserSettingsResponse
            {
                UserId = UserId,
                Settings = new List<NotificationSettingModel>
                {
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Push,
                        Event = NotificationEvent.ArticleCreated,
                        Enabled = true
                    }
                }
            };

            UserProfileRequest req = null;
            service.Setup(x => x.CreateUserSettingsAsync(It.Is<UserProfileRequest>(sr => sr == request && sr.UserId == UserId)))
                .Callback<UserProfileRequest>(r => req = r)
                .ReturnsAsync(response)
                .Verifiable();

            var controller = new SettingsController(_logger, service.Object).WithUser();
            var result = await controller.CreateUserSettingsAsync(request);

            service.Verify();
            Assert.NotNull(req);
            Assert.Equal(UserId, req.UserId);

            var res = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal(response, res.Value);
            Assert.IsType<UserSettingsResponse>(res.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetUserSettingsTest()
        {
            var service = new Mock<ISettingsService>();
           
            var response = new UserProfileResponse
            {
                Email = "alex@",
                FirstName = "alex",
                Language = LanguageName.Fr,
                LastName = "softeq",
                PhoneNumber = "123"
            };

            service.Setup(x => x.GetUserSettingsAsync(It.Is<UserRequest>(sr => sr.UserId == UserId)))
                .ReturnsAsync(response)
                .Verifiable();

            var controller = new SettingsController(_logger, service.Object).WithUser();
            var result = await controller.GetUserSettingsAsync();

            service.Verify();
            var res = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, res.Value);
            Assert.IsType<UserProfileResponse>(res.Value);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateUserSettingsTest()
        {
            var service = new Mock<ISettingsService>();

            var request = new UserProfileRequest
            {
                Email = "alex@",
                FirstName = "alex",
                Language = LanguageName.Fr,
                LastName = "softeq",
                PhoneNumber = "123"
            };

            var response = new UserProfileResponse
            {
                UserId = UserId,
                Email = "alex@",
                FirstName = "alex",
                Language = LanguageName.Fr,
                LastName = "softeq",
                PhoneNumber = "123"
            };

            UserProfileRequest req = null;
            service.Setup(x => x.UpdateUserSettingsAsync(It.Is<UserProfileRequest>(sr => sr == request && sr.UserId == UserId)))
                .Callback<UserProfileRequest>(r => req = r)
                .ReturnsAsync(response)
                .Verifiable();

            var controller = new SettingsController(_logger, service.Object).WithUser();
            var result = await controller.UpdateUserSettingsAsync(request);

            service.Verify();
            var res = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, res.Value);
            Assert.IsType<UserProfileResponse>(res.Value);
        }
    }
}
