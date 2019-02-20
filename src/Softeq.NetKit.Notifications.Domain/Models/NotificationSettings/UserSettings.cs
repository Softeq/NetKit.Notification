// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Notifications.Domain.Models.NotificationSettings
{
    public class UserSettings : UserProfileSettings
    {
        public IList<NotificationSetting> Settings { get; set; }
    }
}
