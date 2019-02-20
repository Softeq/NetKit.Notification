// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Softeq.NetKit.Notifications.Domain.Infrastructure;
using Softeq.NetKit.Notifications.Domain.Models.Localization;

namespace Softeq.NetKit.Notifications.Domain.Models.NotificationSettings
{
    public class UserProfileSettings : Entity<Guid>, ICreated, IUpdated
    {
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public LanguageName Language { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
