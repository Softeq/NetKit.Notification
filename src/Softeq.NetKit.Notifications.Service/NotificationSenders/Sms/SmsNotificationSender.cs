// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Logging;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;
using Softeq.NetKit.Services.SmsNotifications.Abstract;
using Softeq.NetKit.Services.SmsNotifications.Exception;
using System.Threading.Tasks;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Sms
{
    internal class SmsNotificationSender : BaseNotificationSender<ISmsNotification>
    {
        private readonly ISmsNotificationService _sender;

        public SmsNotificationSender(ISmsNotificationService sender,
            IMessageFactory<ISmsNotification> messageFactory,
            IMessageValidatorProvider validatorProvider,
            ILoggerFactory loggerFactory) : base(messageFactory, validatorProvider, loggerFactory)
        {
            _sender = sender;
        }

        protected override async Task PerformSending(ISmsNotification message, UserSettings settings, NotificationSendingResult result)
        {
            try
            {
                await _sender.SendAsync(message);
            }
            catch (SmsSenderException ex)
            {
                Logger.LogError(ex, "Exception while sending sms notification");
                result.Errors.Add("Unspecified error");
            }
        }

        protected override NotificationType SenderType => NotificationType.SMS;

        protected override bool ShouldSend(NotificationEvent @event, UserSettings settings, NotificationSendingResult result)
        {
            if (string.IsNullOrWhiteSpace(settings.PhoneNumber))
            {
                result.Errors.Add("User phone number is not configured");
                return false;
            }

            return base.ShouldSend(@event, settings, result);
        }
    }
}
