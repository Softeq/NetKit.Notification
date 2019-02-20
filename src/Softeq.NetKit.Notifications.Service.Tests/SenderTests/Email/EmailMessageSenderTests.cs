// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Logging;
using Moq;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Messages;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;
using Softeq.NetKit.Services.EmailNotifications.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Models;
using Softeq.NetKit.Services.EmailNotifications.Exception;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.SenderTests.Email
{
    public class EmailMessageSenderTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private static readonly MessageValidatorProvider Provider = new MessageValidatorProvider();

        public EmailMessageSenderTests()
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
                Email = "testEmail",
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Email, NotificationEvent.PackageArrived, false) }
            };

            var emailSender = new Mock<IEmailNotificationService>();
            var factory = new Mock<IMessageFactory<IEmailNotification>>();

            var sender = new EmailNotificationSender(emailSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(new NotificationMessage
            {
                Event = NotificationEvent.PackageArrived,
                Parameters = new Dictionary<string, object>()
            }, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Skipped, result.Status);

            result = await sender.SendAsync(new NotificationMessage
            {
                Event = NotificationEvent.CommentLiked,
                Parameters = new Dictionary<string, object>()
            }, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Skipped, result.Status);

            emailSender.Verify(x => x.SendAsync(It.IsAny<IEmailNotification>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenValidationFailedThenExecutionFailed()
        {
            var settings = new UserSettings
            {
                Email = "testEmail",
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Email, NotificationEvent.PackageArrived, true) }
            };

            var message = new NotificationMessage
            {
                Event = NotificationEvent.PackageArrived,
                Parameters = new Dictionary<string, object>()
            };

            var emailSender = new Mock<IEmailNotificationService>();
            var factory = new Mock<IMessageFactory<IEmailNotification>>();
            factory.Setup(x => x.Create(It.Is<NotificationMessage>(notificationMessage => notificationMessage == message),
                It.Is<UserSettings>(userSettings => userSettings == settings)))
                .Returns(new PackageArrivedEmailMessage("", "", new PackageArrivedEmailModel()));

            var sender = new EmailNotificationSender(emailSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(message, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Failed, result.Status);
            Assert.True(result.Errors.Any());

            emailSender.Verify(x => x.SendAsync(It.IsAny<IEmailNotification>()), Times.Never);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenEmailIsNotConfiguredThenErrorReturned()
        {
            var settings = new UserSettings
            {
                Email = "",
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Email, NotificationEvent.PackageArrived, true) }
            };

            var message = new NotificationMessage
            {
                Event = NotificationEvent.PackageArrived,
                Parameters = new Dictionary<string, object>()
            };

            var emailSender = new Mock<IEmailNotificationService>();
            var factory = new Mock<IMessageFactory<IEmailNotification>>();

            var sender = new EmailNotificationSender(emailSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(message, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Failed, result.Status);
            Assert.True(result.Errors.Any());

            emailSender.Verify(x => x.SendAsync(It.IsAny<IEmailNotification>()), Times.Never);
        }
       
        [Fact]
        [Trait("Category", "Unit")]
        public async Task SendMessageExceptionHandling()
        {
            var settings = new UserSettings
            {
                Email = "testEmail",
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Email, NotificationEvent.PackageArrived, true) }
            };

            var message = new NotificationMessage
            {
                Event = NotificationEvent.PackageArrived,
                Parameters = new Dictionary<string, object>()
            };

            var factory = new Mock<IMessageFactory<IEmailNotification>>();
            factory.Setup(x => x.Create(It.Is<NotificationMessage>(notificationMessage => notificationMessage == message),
                    It.Is<UserSettings>(userSettings => userSettings == settings)))
                .Returns(validMessage)
                .Verifiable();

            var emailSender = new Mock<IEmailNotificationService>();
            emailSender.Setup(x => x.SendAsync(It.IsAny<IEmailNotification>()))
                .ThrowsAsync(new EmailSenderException("error"));

            var sender = new EmailNotificationSender(emailSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(message, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Failed, result.Status);
            Assert.True(result.Errors.Any());

            factory.Verify();
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task MessageSentSuccessfully()
        {
            var settings = new UserSettings
            {
                Email = "testEmail",
                Settings = new List<NotificationSetting> { new NotificationSetting(NotificationType.Email, NotificationEvent.PackageArrived, true) }
            };

            var message = new NotificationMessage
            {
                Event = NotificationEvent.PackageArrived,
                Parameters = new Dictionary<string, object>()
            };

            var factory = new Mock<IMessageFactory<IEmailNotification>>();
            factory.Setup(x => x.Create(It.Is<NotificationMessage>(notificationMessage => notificationMessage == message),
                    It.Is<UserSettings>(userSettings => userSettings == settings)))
                .Returns(validMessage);

            var emailSender = new Mock<IEmailNotificationService>();

            var sender = new EmailNotificationSender(emailSender.Object, factory.Object, Provider, _loggerFactory);
            var result = await sender.SendAsync(message, settings);

            Assert.NotNull(result);
            Assert.Equal(NotificationSendingStatus.Success, result.Status);
            Assert.False(result.Errors.Any());
        }

        private static PackageArrivedEmailMessage validMessage = new PackageArrivedEmailMessage("Alex", "alex@mail",
            new PackageArrivedEmailModel
            {
                OrderId = Guid.NewGuid().ToString(),
                TrackingNumber = Guid.NewGuid().ToString(),
                UserName = "Alex"
            })
        {
            HtmlTemplate = "template",
            BaseHtmlTemplate = "template",
            Subject = "subject"
        };
    }
}
