// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Domain.Models.NotificationSettings
{
    public class NotificationSetting
    {
        public NotificationSetting()
        {
        }

        public NotificationSetting(NotificationType type, NotificationEvent @event) : this(type, @event, true)
        {
        }

        public NotificationSetting(NotificationType type, NotificationEvent @event, bool enabled)
        {
            Type = type;
            Enabled = enabled;
            Event = @event;
        }

        public NotificationType Type { get; set; }
        public NotificationEvent Event { get; set; }
        public bool Enabled { get; set; }
    }
}
