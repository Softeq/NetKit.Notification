// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Softeq.NetKit.Services.EmailNotifications.Models;

namespace Softeq.NetKit.Services.EmailNotifications.EmailSender
{
    public interface IEmailSender
    {
        Task SendAsync(SendEmailDto email);
    }
}
