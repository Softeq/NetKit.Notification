// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Logging;
using Moq;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Messages;
using Softeq.NetKit.Services.PushNotifications.Abstractions;
using Softeq.NetKit.Services.PushNotifications.Exception;
using Softeq.NetKit.Services.PushNotifications.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.SenderTests.Push
{
    public class PushMessageSenderTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private static readonly MessageValidatorProvider Provider = new MessageValidatorProvider();

        public PushMessageSenderTests()
        {
            var logger = new Mock<ILogger>();
            var factoryMock = new Mock<ILoggerFactory>();
            factoryMock.Setup(x => x.CreateLogger(It.IsAny<string>()))
                .Returns(logger.Object);
            _loggerFactory = factoryMock.Object;
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenSettingDisabledOrUnsupportedThenExecutionSkipped()
        {
            var settings = new UserSettings
            {
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Push, NotificationEvent.ArticleCreated, false) }
            };

            var pushSender = new Mock<IPushNotificationSender>();
            var factory = new Mock<IMessageFactory<PushNotificationMessage>>();

            var sender = new PushNotificationSender(pushSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(new NotificationMessage
            {
                Event = NotificationEvent.ArticleCreated,
                Parameters = new Dictionary<string, object>()
            }, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Skipped, result.Status);

            result = await sender.SendAsync(new NotificationMessage
            {
                Event = NotificationEvent.PackageArrived,
                Parameters = new Dictionary<string, object>()
            }, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Skipped, result.Status);

            pushSender.Verify(x => x.SendAsync(It.IsAny<PushNotificationMessage>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenValidationFailedThenExecutionFailed()
        {
            var settings = new UserSettings
            {
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Push, NotificationEvent.ArticleCreated, true) }
            };

            var message = new NotificationMessage
            {
                Event = NotificationEvent.ArticleCreated,
                Parameters = new Dictionary<string, object>()
            };

            var pushSender = new Mock<IPushNotificationSender>();
            var factory = new Mock<IMessageFactory<PushNotificationMessage>>();
            factory.Setup(x => x.Create(It.Is<NotificationMessage>(notificationMessage => notificationMessage == message),
                It.Is<UserSettings>(userSettings => userSettings == settings)))
                .Returns(new ArticleCreatedPushMessage());

            var sender = new PushNotificationSender(pushSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(message, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Failed, result.Status);
            Assert.True(result.Errors.Any());

            pushSender.Verify(x => x.SendAsync(It.IsAny<PushNotificationMessage>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenSendingIsUnsuccessfulThenErrorReturned()
        {
            var settings = new UserSettings
            {
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Push, NotificationEvent.ArticleCreated, true) }
            };

            var message = new NotificationMessage
            {
                Event = NotificationEvent.ArticleCreated,
                Parameters = new Dictionary<string, object>()
            };

            var factory = new Mock<IMessageFactory<PushNotificationMessage>>();
            factory.Setup(x => x.Create(It.Is<NotificationMessage>(notificationMessage => notificationMessage == message),
                    It.Is<UserSettings>(userSettings => userSettings == settings)))
                .Returns(new ArticleCreatedPushMessage { ArticleId = Guid.NewGuid() });

            var pushSender = new Mock<IPushNotificationSender>();
            pushSender.Setup(x => x.SendAsync(It.IsAny<PushNotificationMessage>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var sender = new PushNotificationSender(pushSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(message, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Failed, result.Status);
            Assert.True(result.Errors.Any());
        }

        [Theory]
        [Trait("Category", "Unit")]
        [MemberData(nameof(SendingExceptions))]
        public async Task SendMessageExceptionHandling(Exception ex)
        {
            var settings = new UserSettings
            {
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Push, NotificationEvent.ArticleCreated, true) }
            };

            var message = new NotificationMessage
            {
                Event = NotificationEvent.ArticleCreated,
                Parameters = new Dictionary<string, object>()
            };

            var factory = new Mock<IMessageFactory<PushNotificationMessage>>();
            factory.Setup(x => x.Create(It.Is<NotificationMessage>(notificationMessage => notificationMessage == message),
                    It.Is<UserSettings>(userSettings => userSettings == settings)))
                .Returns(new ArticleCreatedPushMessage { ArticleId = Guid.NewGuid() });

            var pushSender = new Mock<IPushNotificationSender>();
            pushSender.Setup(x => x.SendAsync(It.IsAny<PushNotificationMessage>(), It.IsAny<string>()))
                .ThrowsAsync(ex);

            var sender = new PushNotificationSender(pushSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(message, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Failed, result.Status);
            Assert.True(result.Errors.Any());
        }

        public static IEnumerable<object[]> SendingExceptions =>
            new List<object[]>
            {
                new []{new PushNotificationException("error")},
                new []{new NetKit.Services.PushNotifications.Exception.ValidationException("error")}
            };

        [Fact]
        [Trait("Category", "Unit")]
        public async Task MessageSentSuccessfully()
        {
            var settings = new UserSettings
            {
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Push, NotificationEvent.ArticleCreated, true) }
            };

            var message = new NotificationMessage
            {
                Event = NotificationEvent.ArticleCreated,
                Parameters = new Dictionary<string, object>()
            };

            var factory = new Mock<IMessageFactory<PushNotificationMessage>>();
            factory.Setup(x => x.Create(It.Is<NotificationMessage>(notificationMessage => notificationMessage == message),
                    It.Is<UserSettings>(userSettings => userSettings == settings)))
                .Returns(new ArticleCreatedPushMessage { ArticleId = Guid.NewGuid() });

            var pushSender = new Mock<IPushNotificationSender>();
            pushSender.Setup(x => x.SendAsync(It.IsAny<PushNotificationMessage>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var sender = new PushNotificationSender(pushSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(message, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Success, result.Status);
            Assert.False(result.Errors.Any());
        }
    }
}
