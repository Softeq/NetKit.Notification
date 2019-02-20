// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using SendGrid;
using SendGrid.Helpers.Mail;
using Softeq.NetKit.Services.EmailNotifications.Exception;
using Softeq.NetKit.Services.EmailNotifications.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Softeq.NetKit.Services.EmailNotifications.EmailSender
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly SendGridClientConfiguration _configuration;

        public SendGridEmailSender(SendGridClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendAsync(SendEmailDto email)
        {
            try
            {
                var client = new SendGridClient(_configuration.ApiKey);
                var message = CreateMessage(email);

                var response = await client.SendEmailAsync(message);
                if (response.StatusCode != HttpStatusCode.Accepted)
                {
                    var body = await response.DeserializeResponseBodyAsync(response.Body);
                    throw new EmailSenderException("Error while sending email", body);
                }
            }
            catch (System.Exception e) when (!(e is EmailSenderException))
            {
                throw new EmailSenderException("Error while sending email", e);
            }
        }

        private SendGridMessage CreateMessage(SendEmailDto email)
        {
            var msg = new SendGridMessage
            {
                From = new EmailAddress(
                    string.IsNullOrWhiteSpace(email.FromEmail) ? _configuration.FromEmail : email.FromEmail,
                    string.IsNullOrWhiteSpace(email.FromName) ? _configuration.FromName : email.FromName),
                Subject = email.Subject,
                PlainTextContent = email.Text,
                HtmlContent = email.HtmlText
            };

            foreach (var recipient in email.Recipients)
            {
                switch (recipient.Type)
                {
                    case EmailDeliveryTypeEnum.Regular:
                        msg.AddTo(new EmailAddress(recipient.Email, recipient.Name));
                        break;
                    case EmailDeliveryTypeEnum.Personal:
                        msg.Personalizations = msg.Personalizations ?? new List<Personalization>();
                        msg.Personalizations.Add(new Personalization { Tos = new List<EmailAddress> { new EmailAddress(recipient.Email, recipient.Name) } });
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported delivery type");
                }
            }

            return msg;
        }
    }
}
