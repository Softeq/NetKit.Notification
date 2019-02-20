// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions
{
    public interface INotificationSender
    {
        Task<NotificationSendingResult> SendAsync(NotificationMessage message, UserSettings settings);
    }
}
