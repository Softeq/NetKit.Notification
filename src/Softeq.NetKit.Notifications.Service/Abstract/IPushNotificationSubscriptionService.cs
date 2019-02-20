// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Service.TransportModels.PushNotification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request;

namespace Softeq.NetKit.Notifications.Service.Abstract
{
    public interface IPushNotificationSubscriptionService
    {
        Task UnsubscribeDeviceAsync(PushDeviceRequest model);
        Task UnsubscribeUserAsync(UserRequest model);
        Task CreateOrUpdateSubscriptionAsync(PushDeviceRequest model);
    }
}
