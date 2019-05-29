// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Services.SmsNotifications.Abstract;
using Softeq.NetKit.Services.SmsNotifications.Exception;
using Softeq.NetKit.Services.SmsNotifications.Models;
using Softeq.NetKit.Services.SmsNotifications.SmsSender;
using System;
using System.Collections.Generic;
using System.Linq;
using Task = System.Threading.Tasks.Task;

namespace Softeq.NetKit.Services.SmsNotifications
{
    public class SmsNotificationService : ISmsNotificationService
    {
        private readonly ISmsSender _smsSender;
        private const string NotAllSmsWereSentExceptionMessage = "Not all messages were sent!";

        public SmsNotificationService(ISmsSender smsSender)
        {
            _smsSender = smsSender;
        }

        public async Task SendAsync(ISmsNotification message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var errors = new Dictionary<string, dynamic>();

            foreach (var recipientNumber in message.RecipientPhoneNumbers)
            {
                try
                {
                    await _smsSender.SendAsync(new SmsDto()
                    {
                        Text = message.Text,
                        RecipientPhoneNumber = recipientNumber
                    });
                }
                catch (SmsSenderException exception)
                {
                    errors.Add(exception.Message, exception.InnerException);
                }
            }

            if (errors.Any())
            {
                throw new SmsSenderException(NotAllSmsWereSentExceptionMessage, errors);
            }
        }
    }
}
