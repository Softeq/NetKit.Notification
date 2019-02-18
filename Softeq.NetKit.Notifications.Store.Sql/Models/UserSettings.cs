// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Softeq.NetKit.Notifications.Domain.Infrastructure;
using Softeq.NetKit.Notifications.Domain.Models;
using Softeq.NetKit.Notifications.Domain.Models.Localization;

namespace Softeq.NetKit.Notifications.Store.Sql.Models
{
    public class UserSettings : Entity<Guid>, ICreated, IUpdated
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public LanguageName Language { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        public virtual IList<NotificationSetting> Settings { get; set; }
        public virtual IList<NotificationRecord> NotificationRecords { get; set; }
    }
}
