// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response
{
    public class NotificationSetResponse
    {
        /// <summary>
        /// The size of this page.
        /// </summary>
        public int ItemsCount { get; set; }

        /// <summary>
        /// The records this page represents.
        /// </summary>
        public IEnumerable<NotificationResponse> Results { get; set; }
    }
}
