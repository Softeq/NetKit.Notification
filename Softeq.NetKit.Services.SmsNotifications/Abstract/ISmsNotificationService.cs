// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Services.SmsNotifications.Abstract
{
    public interface ISmsNotificationService
    {
        Task SendAsync(ISmsNotification message);
    }
}
