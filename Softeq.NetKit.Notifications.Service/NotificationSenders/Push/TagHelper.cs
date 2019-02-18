// Developed by Softeq Development Corporation
// http://www.softeq.com

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push
{
    internal static class TagHelper
    {
        public static string GetUserTag(string userId)
        {
            return $"userId:{userId}";
        }
    }
}
