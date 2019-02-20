// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Notifications.Service.TransportModels.Shared.Request
{
    public class UserRequest
    {
        public UserRequest(string userId)
        {
            UserId = userId;
        }
        public string UserId { get; set; }
    }
}
