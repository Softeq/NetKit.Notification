// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Moq;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.SenderTests.Email
{
    public class EmailMessageFactoryTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void WhenEventIsNotSupportedThenErrorIsThrown()
        {
            var provider = new Mock<IEmailMessageResourceProvider>();

            var factory = new EmailMessageFactory(provider.Object);
            Assert.Throws<InvalidOperationException>(() => factory.Create(new NotificationMessage
            {
                Event = NotificationEvent.CommentLiked
            }, new UserSettings()));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WhenEventIsSupportedThenMessageReturned()
        {
            var provider = new Mock<IEmailMessageResourceProvider>();
            var supportedEvents = NotificationEventConfiguration.Config[NotificationType.Email];

            var factory = new EmailMessageFactory(provider.Object);

            foreach (var ev in supportedEvents)
            {
                var message = factory.Create(new NotificationMessage
                {
                    Parameters = new Dictionary<string, object>(),
                    Event = ev.Event
                }, new UserSettings());

                Assert.NotNull(message);
            }
        }
    }
}
