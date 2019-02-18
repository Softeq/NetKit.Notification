// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Models
{
    public class NotificationSetting
    {
        public NotificationEvent Event { get; set; }
        public NotificationType Type { get; set; }
        public bool Enabled { get; set; }
    }
}
