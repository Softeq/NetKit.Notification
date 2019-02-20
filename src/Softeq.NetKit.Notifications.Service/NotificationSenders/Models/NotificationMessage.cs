// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Models
{
    public class NotificationMessage
    {
        public NotificationEvent Event { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
