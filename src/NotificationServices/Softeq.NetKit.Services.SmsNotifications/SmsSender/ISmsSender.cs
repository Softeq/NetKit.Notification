// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Services.SmsNotifications.Models;

namespace Softeq.NetKit.Services.SmsNotifications.SmsSender
{
    public interface ISmsSender
    {
        Task SendAsync(SmsDto sms);
    }
}
