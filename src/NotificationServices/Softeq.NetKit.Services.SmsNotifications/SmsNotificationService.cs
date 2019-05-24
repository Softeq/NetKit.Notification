// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Services.SmsNotifications.Abstract;
using System;
using System.Threading.Tasks;
using Softeq.NetKit.Services.SmsNotifications.Models;
using Softeq.NetKit.Services.SmsNotifications.SmsSender;

namespace Softeq.NetKit.Services.SmsNotifications
{
    public class SmsNotificationService : ISmsNotificationService
    {
        private readonly ISmsSender _smsSender;
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

            foreach (var recipientNumber in message.RecipientPhoneNumbers)
            {
                await _smsSender.SendAsync(new SmsDto()
                {
                    Text = message.Text,
                    RecipientPhoneNumber = recipientNumber
                });
            }
        }
    }
}
