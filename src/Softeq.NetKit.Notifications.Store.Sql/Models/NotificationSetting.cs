// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Notifications.Domain.Models;

namespace Softeq.NetKit.Notifications.Store.Sql.Models
{
    public class NotificationSetting : Entity<Guid>
    {
        public Guid UserSettingsId { get; set; }
        public string Event { get; set; }
        public string Type { get; set; }
        public bool Enabled { get; set; }

        public virtual UserSettings UserSettings { get; set; }
    }
}
