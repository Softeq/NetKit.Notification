// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request
{
    public class GetNotificationRequest : UserRequest
    {
        public GetNotificationRequest(string userId, Guid notificationId) : base(userId)
        {
            NotificationId = notificationId;
        }

        public Guid NotificationId { get; set; }
    }
}
