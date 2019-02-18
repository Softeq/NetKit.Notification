using System.Collections.Generic;
using Softeq.NetKit.Components.EventBus.Events;
using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Service.EventBus.Events
{
    public class ArticleCreatedNotificationEvent : IntegrationEvent
    {
        public string RecipientUserId { get; set; }
        public NotificationEvent EventType { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
