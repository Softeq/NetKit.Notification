// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Linq;
using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Models
{
    public class NotificationSendingResult
    {
        private NotificationSendingStatus _status;

        public NotificationSendingResult(NotificationType type)
        {
            _status = NotificationSendingStatus.Success;
            SenderType = type;
            Errors = new List<string>();
        }

        public NotificationType SenderType { get; }
        public List<string> Errors { get; }

        public NotificationSendingStatus Status => Errors.Any() ? NotificationSendingStatus.Failed : _status;

        internal void Skip()
        {
            _status = NotificationSendingStatus.Skipped;
        }
    }

    public enum NotificationSendingStatus
    {
        Success,
        Failed,
        Skipped
    }
}
