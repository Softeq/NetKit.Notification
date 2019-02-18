// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using Softeq.NetKit.Notifications.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Notifications.Web.Tests.UnitTests
{
    public class NotificationHistoryControllerTests : UnitTestBase
    {
        private readonly ILogger<NotificationHistoryController> _logger = new Mock<ILogger<NotificationHistoryController>>().Object;

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteNotificationsTest()
        {
            var service = new Mock<INotificationHistoryService>();
            UserRequest req = null;
            service.Setup(x => x.DeleteAllAsync(It.IsAny<UserRequest>()))
                .Callback<UserRequest>(request => req = request)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var controller = new NotificationHistoryController(_logger, service.Object).WithUser();
            var result = await controller.DeleteNotificationsAsync();

            service.Verify();
            Assert.NotNull(req);
            Assert.Equal(UserId, req.UserId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetByIdTest()
        {
            var notificationId = Guid.NewGuid();
            var service = new Mock<INotificationHistoryService>();
            var response = new NotificationResponse(NotificationEvent.ArticleCreated)
            {
                OwnerUserId = UserId
            };

            GetNotificationRequest req = null;
            service.Setup(x => x.GetAsync(It.IsAny<GetNotificationRequest>()))
                .Callback<GetNotificationRequest>(request => req = request)
                .ReturnsAsync(response)
                .Verifiable();

            var controller = new NotificationHistoryController(_logger, service.Object).WithUser();
            var result = await controller.GetNotificationByIdAsync(notificationId);

            service.Verify();
            Assert.NotNull(req);
            Assert.Equal(UserId, req.UserId);
            Assert.Equal(notificationId, req.NotificationId);

            var res = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, res.Value);
            Assert.IsType<NotificationResponse>(res.Value);
        }

        [Theory]
        [Trait("Category", "Unit")]
        [InlineData(30, "2009-11-25", null)]
        [InlineData(50, null, "2009-11-25")]
        [InlineData(50, "2009-11-25", "2009-11-26")]
        public async Task ListTest(int pageSize, string startTime, string endTime)
        {
            var expectedStartTime = startTime == null ? (DateTimeOffset?)null: DateTimeOffset.Parse(startTime, CultureInfo.InvariantCulture);
            var expectedEndTime = endTime == null ? (DateTimeOffset?) null : DateTimeOffset.Parse(endTime, CultureInfo.InvariantCulture);
            var service = new Mock<INotificationHistoryService>();
            var response = new NotificationSetResponse
            {
                Results = new List<NotificationResponse>
                {
                    new NotificationResponse(NotificationEvent.ArticleCreated)
                    {
                        OwnerUserId = UserId
                    },
                    new NotificationResponse(NotificationEvent.CommentLiked)
                    {
                        OwnerUserId = UserId
                    }
                },
                ItemsCount = 2
            };

            GetNotificationsRequest req = null;
            service.Setup(x => x.ListAsync(It.IsAny<GetNotificationsRequest>()))
                .Callback<GetNotificationsRequest>(request => req = request)
                .ReturnsAsync(response)
                .Verifiable();

            var controller = new NotificationHistoryController(_logger, service.Object).WithUser();
            var result = await controller.GetNotificationsAsync(pageSize, startTime, endTime);

            service.Verify();
            Assert.NotNull(req);
            Assert.Equal(UserId, req.UserId);
            Assert.Equal(pageSize, req.PageSize);
            Assert.Equal(expectedEndTime, req.Options.EndTime);
            Assert.Equal(expectedStartTime, req.Options.StartTime);

            var res = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<NotificationSetResponse>(res.Value);
            Assert.Equal(response, res.Value);
        }
    }
}
