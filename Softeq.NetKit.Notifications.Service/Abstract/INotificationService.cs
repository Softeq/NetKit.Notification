// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;

namespace Softeq.NetKit.Notifications.Service.Abstract
{
    public interface INotificationService
    {
        Task<SendNotificationResponse> PostAsync(SendNotificationRequest request);
    }
}
