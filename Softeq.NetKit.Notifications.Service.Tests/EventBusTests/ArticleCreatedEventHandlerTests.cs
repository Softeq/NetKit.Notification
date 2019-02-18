// Developed by Softeq Development Corporation
// http://www.softeq.com

using Moq;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.EventBus.Events;
using Softeq.NetKit.Notifications.Service.EventBus.Handlers;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.EventBusTests
{
    public class ArticleCreatedEventHandlerTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenNewEventReceivedThenNotificationServiceCalled()
        {
            var service = new Mock<INotificationService>();
            SendNotificationRequest saveDto = null;

            service.Setup(c => c.PostAsync(It.IsAny<SendNotificationRequest>()))
                .Callback<SendNotificationRequest>((dto) => saveDto = dto)
                .Returns(Task.FromResult(new SendNotificationResponse()));

            var @event = new ArticleCreatedNotificationEvent
            {
                EventType = NotificationEvent.ArticleCreated,
                RecipientUserId = Guid.NewGuid().ToString(),
                Parameters = new Dictionary<string, object> { { "ArticleId", Guid.NewGuid() } }
            };

            var handler = new ArticleCreatedNotificationEventHandler(service.Object);
            await handler.Handle(@event);

            service.Verify(x=>x.PostAsync(saveDto), Times.Once);
            Assert.NotNull(saveDto);
            Assert.Equal(@event.EventType, saveDto.EventType);
            Assert.Equal(@event.RecipientUserId, saveDto.RecipientUserId);
            Assert.Equal(@event.Parameters, saveDto.Parameters);
        }
    }
}
