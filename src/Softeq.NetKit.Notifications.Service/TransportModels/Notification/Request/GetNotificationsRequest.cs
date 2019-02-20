// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models;

namespace Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request
{
    public class GetNotificationsRequest
    {
        public GetNotificationsRequest(string userId, FilterOptions options, int pageSize)
        {
            UserId = userId;
            Options = options;
            PageSize = pageSize;
        }

        public string UserId { get; }
        public int PageSize { get; set; }
        public FilterOptions Options { get; }
    }
}
