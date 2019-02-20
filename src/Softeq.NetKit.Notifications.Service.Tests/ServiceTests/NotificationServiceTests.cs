// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;
using Softeq.NetKit.Notifications.Service.Services;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.ServiceTests
{
    public class NotificationServiceTests : UnitTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task ValidationFailedTest()
        {
            var store = new Mock<ISettingsDataStore>();
            var historyStore = new Mock<INotificationHistoryDataStore>();
            var sender = new Mock<INotificationSender>();
            var senders = new List<INotificationSender> { sender.Object };

            var service = new NotificationService(store.Object, senders, historyStore.Object, DefaultMapper);
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.PostAsync(null));

            store.Verify(x => x.FindAsync(It.IsAny<string>()), Times.Never);
            historyStore.Verify(x => x.SaveAsync(It.IsAny<NotificationRecord>()), Times.Never);
            sender.Verify(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task MissingUserSettingsTest()
        {
            var userId = Guid.NewGuid().ToString();
            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId))).ReturnsAsync((UserSettings)null);
            var historyStore = new Mock<INotificationHistoryDataStore>();
            var sender = new Mock<INotificationSender>();
            var senders = new List<INotificationSender> { sender.Object };

            var service = new NotificationService(store.Object, senders, historyStore.Object, DefaultMapper);
            await Assert.ThrowsAsync<NotFoundException>(() => service.PostAsync(new SendNotificationRequest
            {
                EventType = NotificationEvent.ArticleCreated,
                RecipientUserId = userId
            }));

            historyStore.Verify(x => x.SaveAsync(It.IsAny<NotificationRecord>()), Times.Never);
            sender.Verify(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenNoSuccessfulSendsThenHistoryRecordNotCreatedTest()
        {
            var userId = Guid.NewGuid().ToString();
            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId)))
                .ReturnsAsync(new UserSettings { UserId = userId });
            var historyStore = new Mock<INotificationHistoryDataStore>();

            var failedResult1 = new NotificationSendingResult(NotificationType.Email);
            failedResult1.Errors.Add("Error1");

            var sender1 = new Mock<INotificationSender>();
            sender1.Setup(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()))
                .ReturnsAsync(failedResult1);

            var failedResult2 = new NotificationSendingResult(NotificationType.Push);
            failedResult2.Errors.Add("Error2");

            var sender2 = new Mock<INotificationSender>();
            sender2.Setup(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()))
                .ReturnsAsync(failedResult2);

            var senders = new List<INotificationSender> { sender1.Object, sender2.Object };

            var service = new NotificationService(store.Object, senders, historyStore.Object, DefaultMapper);
            var result = await service.PostAsync(new SendNotificationRequest
            {
                EventType = NotificationEvent.ArticleCreated,
                RecipientUserId = userId
            });

            Assert.NotNull(result);

            historyStore.Verify(x => x.SaveAsync(It.IsAny<NotificationRecord>()), Times.Never);
            sender1.Verify(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()), Times.Once);
            sender2.Verify(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()), Times.Once);
            Assert.Null(result.NotificationRecordId);
            Assert.Equal(2, result.Results.Count);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenSenderIsSkippedThenNotAddedToResponseTest()
        {
            var userId = Guid.NewGuid().ToString();
            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId)))
                .ReturnsAsync(new UserSettings { UserId = userId });
            var historyStore = new Mock<INotificationHistoryDataStore>();

            var skippedResult = new NotificationSendingResult(NotificationType.Email);
            skippedResult.Skip();
            var sender1 = new Mock<INotificationSender>();
            sender1.Setup(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()))
                .ReturnsAsync(skippedResult);

            var failedResult = new NotificationSendingResult(NotificationType.Push);
            failedResult.Errors.Add("Error2");

            var sender2 = new Mock<INotificationSender>();
            sender2.Setup(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()))
                .ReturnsAsync(failedResult);

            var senders = new List<INotificationSender> { sender1.Object, sender2.Object };

            var service = new NotificationService(store.Object, senders, historyStore.Object, DefaultMapper);
            var result = await service.PostAsync(new SendNotificationRequest
            {
                EventType = NotificationEvent.ArticleCreated,
                RecipientUserId = userId
            });

            Assert.NotNull(result);

            historyStore.Verify(x => x.SaveAsync(It.IsAny<NotificationRecord>()), Times.Never);
            sender1.Verify(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()), Times.Once);
            sender2.Verify(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()), Times.Once);
            Assert.Null(result.NotificationRecordId);
            Assert.Equal(1, result.Results.Count);
            Assert.DoesNotContain(skippedResult, result.Results);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenSenderSucceedsThenRecordCreatedTest()
        {
            var userId = Guid.NewGuid().ToString();
            var request = new SendNotificationRequest
            {
                EventType = NotificationEvent.ArticleCreated,
                RecipientUserId = userId,
                Parameters = new Dictionary<string, object> { { "testKey", "testValue" } }
            };

            var store = new Mock<ISettingsDataStore>();
            store.Setup(x => x.FindAsync(It.Is<string>(s => s == userId)))
                .ReturnsAsync(new UserSettings
                {
                    Id = Guid.NewGuid(),
                    UserId = userId
                });

            NotificationRecord newRecord = null;
            var historyStore = new Mock<INotificationHistoryDataStore>();
            historyStore.Setup(x => x.SaveAsync(It.IsAny<NotificationRecord>()))
                .Callback<NotificationRecord>(record =>
                {
                    newRecord = record;
                    newRecord.Id = Guid.NewGuid();
                })
                .ReturnsAsync(() => newRecord);

            var skippedResult = new NotificationSendingResult(NotificationType.Email);
            skippedResult.Skip();
            var sender1 = new Mock<INotificationSender>();
            sender1.Setup(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()))
                .ReturnsAsync(skippedResult);

            var successfulResult = new NotificationSendingResult(NotificationType.Push);
            var sender2 = new Mock<INotificationSender>();
            sender2.Setup(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()))
                .ReturnsAsync(successfulResult);

            var senders = new List<INotificationSender> { sender1.Object, sender2.Object };

            var service = new NotificationService(store.Object, senders, historyStore.Object, DefaultMapper);
            var result = await service.PostAsync(request);

            Assert.NotNull(result);

            historyStore.Verify(x => x.SaveAsync(It.IsAny<NotificationRecord>()), Times.Once);
            sender1.Verify(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()), Times.Once);
            sender2.Verify(x => x.SendAsync(It.IsAny<NotificationMessage>(), It.IsAny<UserSettings>()), Times.Once);
            Assert.NotNull(result.NotificationRecordId);
            Assert.Equal(1, result.Results.Count);
            Assert.DoesNotContain(skippedResult, result.Results);
            Assert.Contains(successfulResult, result.Results);

            Assert.Equal(newRecord.Id, result.NotificationRecordId);
            Assert.Equal(request.RecipientUserId, newRecord.OwnerUserId);
            Assert.Equal(request.Parameters, newRecord.Parameters);
        }
    }
}
