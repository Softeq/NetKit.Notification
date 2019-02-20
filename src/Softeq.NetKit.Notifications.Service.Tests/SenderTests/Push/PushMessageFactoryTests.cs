// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push;
using System;
using System.Collections.Generic;
using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.SenderTests.Push
{
    public class PushMessageFactoryTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public void WhenEventIsNotSupportedThenErrorIsThrown()
        {
            var provider = new Mock<IPushMessageResourceProvider>();

            var factory = new PushMessageFactory(provider.Object);
            Assert.Throws<InvalidOperationException>(() => factory.Create(new NotificationMessage
            {
                Event = NotificationEvent.PackageArrived
            }, new UserSettings()));
        }

        [Fact]
        [Trait("Category", "Unit")]
        public void WhenEventIsSupportedThenMessageReturned()
        {
            var provider = new Mock<IPushMessageResourceProvider>();
            var supportedEvents = NotificationEventConfiguration.Config[NotificationType.Push];

            var factory = new PushMessageFactory(provider.Object);

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
