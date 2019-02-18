// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Services.EmailNotifications.Abstract;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Models
{
    public class ForgotPasswordEmailModel : IEmailTemplateModel
    {
        public string Link { get; set; }

        public string Name { get; set; }
    }
}
