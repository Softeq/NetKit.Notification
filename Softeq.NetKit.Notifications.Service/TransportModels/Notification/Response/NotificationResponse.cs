// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response
{
    public class NotificationResponse
    {
        public NotificationResponse(NotificationEvent @event)
        {
            Event = @event;
        }

        public Guid Id { get; set; }

        public DateTimeOffset Created { get; set; }

        public string OwnerUserId { get; set; }

        public NotificationEvent Event { get; }

        public Dictionary<string, object> Parameters { get; set; }
    }
}
