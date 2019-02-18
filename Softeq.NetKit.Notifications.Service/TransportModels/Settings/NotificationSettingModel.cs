using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Settings
{
    public class NotificationSettingModel
    {
        public NotificationType Type { get; set; }
        public NotificationEvent Event { get; set; }
        public bool Enabled { get; set; }
    }
}