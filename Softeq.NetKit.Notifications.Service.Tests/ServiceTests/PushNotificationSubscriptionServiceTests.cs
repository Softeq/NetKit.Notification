// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Models.PushNotification;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push;
using Softeq.NetKit.Notifications.Service.Services;
using Softeq.NetKit.Notifications.Service.TransportModels.PushNotification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using Softeq.NetKit.Services.PushNotifications.Abstractions;
using Softeq.NetKit.Services.PushNotifications.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.ServiceTests
{
    public class PushNotificationSubscriptionServiceTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ValidationFailedTest()
        {
            var store = new Mock<ISettingsDataStore>();
            var subscriber = new Mock<IPushNotificationSubscriber>();

            var service = new PushNotificationSubscriptionService(store.Object, subscriber.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UnsubscribeDeviceAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UnsubscribeUserAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateOrUpdateSubscriptionAsync(null));

            subscriber.Verify(x => x.UnsubscribeDeviceAsync(It.IsAny<string>()), Times.Never);
            subscriber.Verify(x => x.UnsubscribeByTagAsync(It.IsAny<string>()), Times.Never);
            subscriber.Verify(x => x.CreateOrUpdatePushSubscriptionAsync(It.IsAny<PushSubscriptionRequest>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task MissingUserSettingsTest()
        {
            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.DoesExistAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var subscriber = new Mock<IPushNotificationSubscriber>();

            var service = new PushNotificationSubscriptionService(store.Object, subscriber.Object);
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UnsubscribeDeviceAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.UnsubscribeUserAsync(null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateOrUpdateSubscriptionAsync(null));

            subscriber.Verify(x => x.UnsubscribeDeviceAsync(It.IsAny<string>()), Times.Never);
            subscriber.Verify(x => x.UnsubscribeByTagAsync(It.IsAny<string>()), Times.Never);
            subscriber.Verify(x => x.CreateOrUpdatePushSubscriptionAsync(It.IsAny<PushSubscriptionRequest>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UnsubscribeDeviceTest()
        {
            var userId = Guid.NewGuid().ToString();
            var token = Guid.NewGuid().ToString();
            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.DoesExistAsync(It.Is<string>(s => s == userId)))
                .ReturnsAsync(true);

            var subscriber = new Mock<IPushNotificationSubscriber>();

            var service = new PushNotificationSubscriptionService(store.Object, subscriber.Object);
            await service.UnsubscribeDeviceAsync(new PushDeviceRequest(DevicePlatform.Android, token) { UserId = userId });

            store.Verify(x => x.DoesExistAsync(userId), Times.Once);
            subscriber.Verify(x => x.UnsubscribeDeviceAsync(token), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UnsubscribeUserTest()
        {
            var userId = Guid.NewGuid().ToString();
            var userTag = TagHelper.GetUserTag(userId);
            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.DoesExistAsync(It.Is<string>(s => s == userId)))
                .ReturnsAsync(true);

            var subscriber = new Mock<IPushNotificationSubscriber>();

            var service = new PushNotificationSubscriptionService(store.Object, subscriber.Object);
            await service.UnsubscribeUserAsync(new UserRequest(userId));

            store.Verify(x => x.DoesExistAsync(userId), Times.Once);
            subscriber.Verify(x => x.UnsubscribeByTagAsync(userTag), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task SubscribeUserTest()
        {
            var userId = Guid.NewGuid().ToString();
            var token = Guid.NewGuid().ToString();
            var userTag = TagHelper.GetUserTag(userId);
            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.DoesExistAsync(It.Is<string>(s => s == userId)))
                .ReturnsAsync(true);

            PushSubscriptionRequest request = null;
            var subscriber = new Mock<IPushNotificationSubscriber>();
            subscriber.Setup(x => x.CreateOrUpdatePushSubscriptionAsync(It.IsAny<PushSubscriptionRequest>()))
                .Callback<PushSubscriptionRequest>((dto) => request = dto)
                .Returns(Task.CompletedTask);

            var service = new PushNotificationSubscriptionService(store.Object, subscriber.Object);
            await service.CreateOrUpdateSubscriptionAsync(new PushDeviceRequest(DevicePlatform.Android, token) { UserId = userId });

            store.Verify(x => x.DoesExistAsync(userId), Times.Once);
            subscriber.Verify(x => x.CreateOrUpdatePushSubscriptionAsync(request), Times.Once);

            Assert.Equal(new[] { userTag }, request.Tags);
            Assert.Equal(token, request.DeviceHandle);
            Assert.Equal((int)DevicePlatform.Android, (int)request.Platform);
        }
    }
}
