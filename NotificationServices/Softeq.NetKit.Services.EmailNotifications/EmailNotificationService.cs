// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Services.EmailNotifications.Abstract;
using Softeq.NetKit.Services.EmailNotifications.EmailSender;
using Softeq.NetKit.Services.EmailNotifications.Models;

namespace Softeq.NetKit.Services.EmailNotifications
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailSender _emailService;

        public EmailNotificationService(IEmailSender emailService)
        {
            _emailService = emailService;
        }

        public Task SendAsync(IEmailNotification message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            return _emailService.SendAsync(new SendEmailDto
            {
                Subject = message.FormatSubject(),
                Recipients = message.Recipients,
                Text = message.Text,
                HtmlText = message.GetHtml()
            });
        }
    }
}
