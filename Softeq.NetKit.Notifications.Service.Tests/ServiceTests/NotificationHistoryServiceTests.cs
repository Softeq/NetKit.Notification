// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using Softeq.NetKit.Notifications.Service.Services;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Models;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.ServiceTests
{
    public class NotificationHistoryServiceTests : UnitTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetNotificationValidationFailedTest()
        {
            var store = new Mock<INotificationHistoryDataStore>();

            var service = new NotificationHistoryService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.GetAsync(null));

            store.Verify(x => x.FindAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetMissingNotificationFailedTest()
        {
            var store = new Mock<INotificationHistoryDataStore>();
            store.Setup(x => x.FindAsync(It.IsAny<Guid>()))
                .ReturnsAsync((NotificationRecord)null);

            var id = Guid.NewGuid();
            var service = new NotificationHistoryService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetAsync(new GetNotificationRequest(Guid.NewGuid().ToString(), id)));

            store.Verify(x => x.FindAsync(id), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetInvalidOwnerNotificationFailedTest()
        {
            var id = Guid.NewGuid();
            var record = new NotificationRecord
            {
                OwnerUserId = Guid.NewGuid().ToString(),
                Id = id
            };

            var store = new Mock<INotificationHistoryDataStore>();
            store.Setup(x => x.FindAsync(It.Is<Guid>(guid => guid == id)))
                .ReturnsAsync(record);

            var service = new NotificationHistoryService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<NotFoundException>(() => service.GetAsync(new GetNotificationRequest(Guid.NewGuid().ToString(), id)));

            store.Verify(x => x.FindAsync(id), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task GetNotificationTest()
        {
            var id = Guid.NewGuid();
            var record = new NotificationRecord
            {
                OwnerUserId = Guid.NewGuid().ToString(),
                Id = id,
                Parameters = new Dictionary<string, object>
                {
                    {"testKey", "testValue" }
                },
                Created = DateTimeOffset.UtcNow,
                Event = NotificationEvent.ArticleCreated
            };

            var store = new Mock<INotificationHistoryDataStore>();
            store.Setup(x => x.FindAsync(It.Is<Guid>(guid => guid == id)))
                .ReturnsAsync(record);

            var service = new NotificationHistoryService(store.Object, DefaultMapper);
            var result = await service.GetAsync(new GetNotificationRequest(record.OwnerUserId, id));

            ValidateRecords(record, result);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteAllValidationFailedTest()
        {
            var store = new Mock<INotificationHistoryDataStore>();

            var service = new NotificationHistoryService(store.Object, DefaultMapper);
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.DeleteAllAsync(null));

            store.Verify(x => x.DeleteAllAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DeleteAllSuccessTest()
        {
            var store = new Mock<INotificationHistoryDataStore>();

            var service = new NotificationHistoryService(store.Object, DefaultMapper);
            var userId = Guid.NewGuid().ToString();
            var request = new UserRequest(userId);

            await service.DeleteAllAsync(request);

            store.Verify(x => x.DeleteAllAsync(userId), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task ListRecordsValidationFailedTest()
        {
            var userId = Guid.NewGuid().ToString();
            var results = new List<NotificationRecord>
            {
                new NotificationRecord
                {
                    Event = NotificationEvent.ArticleCreated,
                    OwnerUserId = userId,
                    Id = Guid.NewGuid(),
                    Parameters = new Dictionary<string, object> {{"testKey", "testValue"}}
                },
                new NotificationRecord
                {
                    Event = NotificationEvent.CommentLiked,
                    OwnerUserId = userId,
                    Id = Guid.NewGuid(),
                    Parameters = new Dictionary<string, object> {{"testKey2", "testValue2"}}
                }
            };
            var store = new Mock<INotificationHistoryDataStore>();
            store.Setup(x => x.ListAsync(It.Is<string>(s => s == userId), It.IsAny<int>(), It.IsAny<FilterOptions>()))
                .ReturnsAsync(results);

            var service = new NotificationHistoryService(store.Object, DefaultMapper);
            var response = await service.ListAsync(new GetNotificationsRequest(userId, new FilterOptions(), 5));

            Assert.NotNull(response);
            Assert.Equal(results.Count, response.ItemsCount);
            Assert.All(response.Results, notificationResponse =>
            {
                var item = results.First(x => x.Id == notificationResponse.Id);
                ValidateRecords(item, notificationResponse);
            });
        }

        private static void ValidateRecords(NotificationRecord expected, NotificationResponse actual)
        {
            Assert.NotNull(actual);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Parameters, actual.Parameters);
            Assert.Equal(expected.OwnerUserId, actual.OwnerUserId);
            Assert.Equal(expected.Event, actual.Event);
            Assert.Equal(expected.Created, actual.Created);
        }
    }
}
