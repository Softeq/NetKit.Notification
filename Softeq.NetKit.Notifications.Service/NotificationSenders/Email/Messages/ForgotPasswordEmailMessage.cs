// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Models;
using Softeq.NetKit.Services.EmailNotifications.Abstract;
using Softeq.NetKit.Services.EmailNotifications.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Messages
{
    public class ForgotPasswordEmailMessage : BaseEmailNotification<ForgotPasswordEmailModel>
    {
        public ForgotPasswordEmailMessage(string toEmail, string toName, ForgotPasswordEmailModel model) : base(new RecipientDto(toEmail, toName))
        {
            TemplateModel = model;
        }
    }
}
