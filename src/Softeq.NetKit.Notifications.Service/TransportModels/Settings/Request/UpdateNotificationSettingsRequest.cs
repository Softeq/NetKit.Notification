// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Settings.Request
{
    public class UpdateNotificationSettingsRequest
    {
        public string UserId { get; set; }

        public IList<NotificationSettingModel> Settings { get; set; }
    }
}

