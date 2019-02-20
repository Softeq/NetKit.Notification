using Softeq.NetKit.Components.EventBus.Abstract;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.EventBus.Events;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using System.Threading.Tasks;

namespace Softeq.NetKit.Notifications.Service.EventBus.Handlers
{
    public class ArticleCreatedNotificationEventHandler : IEventHandler<ArticleCreatedNotificationEvent>
    {
        private readonly INotificationService _service;

        public ArticleCreatedNotificationEventHandler(INotificationService service)
        {
            _service = service;
        }

        public Task Handle(ArticleCreatedNotificationEvent @event)
        {
            var model = new SendNotificationRequest
            {
                EventType = @event.EventType,
                Parameters = @event.Parameters,
                RecipientUserId = @event.RecipientUserId
            };
            return _service.PostAsync(model);
        }
    }
}
