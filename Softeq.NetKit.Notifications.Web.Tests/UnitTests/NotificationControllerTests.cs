// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using Softeq.NetKit.Notifications.Web.Controllers;
using Xunit;

namespace Softeq.NetKit.Notifications.Web.Tests.UnitTests
{
    public class NotificationControllerTests : UnitTestBase
    {
        private readonly ILogger<NotificationController> _logger = new Mock<ILogger<NotificationController>>().Object;

        [Fact]
        [Trait("Category", "Unit")]
        public async Task PostNotificationTest()
        {
            var service = new Mock<INotificationService>();
            var response = new SendNotificationResponse
            {
                NotificationRecordId = Guid.NewGuid(),
                Results = new List<NotificationSendingResult>()
            };

            var request = new SendNotificationRequest
            {
                EventType = NotificationEvent.ArticleCreated,
                RecipientUserId = "userId",
                Parameters = new Dictionary<string, object>()
            };

            SendNotificationRequest req = null;
            service.Setup(x => x.PostAsync(It.Is<SendNotificationRequest>(notificationRequest => notificationRequest == request)))
                .Callback<SendNotificationRequest>(r => req = r)
                .ReturnsAsync(response)
                .Verifiable();

            var controller = new NotificationController(_logger, service.Object).WithUser();
            var result = await controller.SendNotificationAsync(request);

            service.Verify();
            Assert.NotNull(req);

            var res = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<SendNotificationResponse>(res.Value);
            Assert.Equal(response, res.Value);
        }
    }
}
