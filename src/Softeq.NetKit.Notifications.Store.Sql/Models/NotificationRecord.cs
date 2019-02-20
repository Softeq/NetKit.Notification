// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Softeq.NetKit.Notifications.Domain.Infrastructure;
using Softeq.NetKit.Notifications.Domain.Models;
using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Store.Sql.Models
{
    public class NotificationRecord : Entity<Guid>, ICreated
    {
        public Guid UserSettingsId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string OwnerUserId { get; set; }
        public NotificationEvent Event { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
        public virtual UserSettings UserSettings { get; set; }
    }
}
