// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.PushNotification;

namespace Softeq.NetKit.Notifications.Service.TransportModels.PushNotification.Request
{
    public class PushDeviceRequest
    {
        public PushDeviceRequest(DevicePlatform platform, string deviceToken)
        {
            Platform = platform;
            DeviceToken = deviceToken;
        }

        public string UserId { get; set; }
        public DevicePlatform Platform { get; }
        public string DeviceToken { get; }
    }
}
