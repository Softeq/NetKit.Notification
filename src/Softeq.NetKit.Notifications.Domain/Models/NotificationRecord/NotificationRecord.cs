// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Softeq.NetKit.Notifications.Domain.Infrastructure;
using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Domain.Models.NotificationRecord
{
    public class NotificationRecord : Entity<Guid>, ICreated
    {
        public Guid UserSettingsId { get; set; }

        public NotificationEvent Event { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the notification owner user identifier.
        /// </summary>
        public string OwnerUserId { get; set; }

        /// <summary>
        /// Gets the notification type that is used for serialization and deserialization.
        /// </summary>
        //public NotificationType Type { get; }

        public DateTimeOffset Created { get; set; }
    }
}
