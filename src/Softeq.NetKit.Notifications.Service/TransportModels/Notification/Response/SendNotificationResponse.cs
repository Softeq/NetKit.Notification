// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response
{
    public class SendNotificationResponse
    {
        public SendNotificationResponse()
        {
            Results = new List<NotificationSendingResult>();    
        }

        public Guid? NotificationRecordId { get; set; }
        public IList<NotificationSendingResult> Results { get; set; }
    }
}
