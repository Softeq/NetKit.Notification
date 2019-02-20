// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request
{
    public class SendNotificationRequest
    {
        public string RecipientUserId { get; set; }
        public NotificationEvent EventType { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
