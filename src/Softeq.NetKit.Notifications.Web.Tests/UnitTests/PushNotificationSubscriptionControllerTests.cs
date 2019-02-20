// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Softeq.NetKit.Notifications.Domain.Models.PushNotification;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.TransportModels.PushNotification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using Softeq.NetKit.Notifications.Web.Controllers;
using Xunit;

namespace Softeq.NetKit.Notifications.Web.Tests.UnitTests
{
    public class PushNotificationSubscriptionControllerTests: UnitTestBase
    {
        private readonly ILogger<PushNotificationSubscriptionController> _logger = new Mock<ILogger<PushNotificationSubscriptionController>>().Object;

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SubscribeDeviceTest()
        {
            var request = new PushDeviceRequest(DevicePlatform.iOS, "token");
            var service = new Mock<IPushNotificationSubscriptionService>();
            PushDeviceRequest req = null;
            service.Setup(x => x.CreateOrUpdateSubscriptionAsync(It.Is<PushDeviceRequest>(deviceRequest => deviceRequest == request)))
                .Callback<PushDeviceRequest>(r => req = r)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var controller = new PushNotificationSubscriptionController(_logger, service.Object).WithUser();
            var result = await controller.SubscribeDeviceAsync(request);

            service.Verify();
            Assert.NotNull(req);
            Assert.Equal(UserId, req.UserId);

            Assert.IsType<OkResult>(result);
            service.Verify();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UnsubscribeUserAsync()
        {
            var service = new Mock<IPushNotificationSubscriptionService>();
            UserRequest req = null;
            service.Setup(x => x.UnsubscribeUserAsync(It.IsAny<UserRequest>()))
                .Callback<UserRequest>(r => req = r)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var controller = new PushNotificationSubscriptionController(_logger, service.Object).WithUser();
            var result = await controller.UnsubscribeUserAsync();

            service.Verify();
            Assert.NotNull(req);
            Assert.Equal(UserId, req.UserId);

            Assert.IsType<NoContentResult>(result);
            service.Verify();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UnsubscribeDeviceAsync()
        {
            var request = new PushDeviceRequest(DevicePlatform.iOS, "token");
            var service = new Mock<IPushNotificationSubscriptionService>();
            PushDeviceRequest req = null;
            service.Setup(x => x.UnsubscribeDeviceAsync(It.Is<PushDeviceRequest>(deviceRequest => deviceRequest == request)))
                .Callback<PushDeviceRequest>(r => req = r)
                .Returns(Task.CompletedTask)
                .Verifiable();

            var controller = new PushNotificationSubscriptionController(_logger, service.Object).WithUser();
            var result = await controller.UnsubscribeDeviceAsync(request);

            service.Verify();
            Assert.NotNull(req);
            Assert.Equal(UserId, req.UserId);

            Assert.IsType<NoContentResult>(result);
            service.Verify();
        }
    }
}
