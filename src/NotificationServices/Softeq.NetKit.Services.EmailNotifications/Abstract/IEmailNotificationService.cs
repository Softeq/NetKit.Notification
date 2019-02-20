// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;

namespace Softeq.NetKit.Services.EmailNotifications.Abstract
{
    public interface IEmailNotificationService
    {
        Task SendAsync(IEmailNotification message);
    }
}