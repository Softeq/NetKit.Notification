// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response
{
    public class NotificationSettingsResponse
    {
        public string UserId { get; set; }
        public IEnumerable<NotificationSettingModel> Settings { get; set; }
    }
}
