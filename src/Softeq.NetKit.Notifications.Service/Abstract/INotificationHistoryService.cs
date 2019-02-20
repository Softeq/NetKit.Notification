// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;

namespace Softeq.NetKit.Notifications.Service.Abstract
{
    public interface INotificationHistoryService
    {
        Task<NotificationResponse> GetAsync(GetNotificationRequest request);
        Task<NotificationSetResponse> ListAsync(GetNotificationsRequest request);
        Task DeleteAllAsync(UserRequest request);
    }
}
