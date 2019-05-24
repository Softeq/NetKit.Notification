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
        protected override NotificationType SenderType => NotificationType.SMS;
        private const string ExceptionWhileSendingSmsMessage = "Exception while sending sms notification";
        private const string UnspecifiedErrorMessage = "Unspecified error";
        private const string PhoneNumberIsNotConfiguredErrorMessage = "User phone number is not configured";

        public SmsNotificationSender(ISmsNotificationService sender,
            IMessageFactory<ISmsNotification> messageFactory,
            IMessageValidatorProvider validatorProvider,
            ILoggerFactory loggerFactory) : base(messageFactory, validatorProvider, loggerFactory)
        {
            _sender = sender;
        }

        protected override async Task PerformSendingAsync(ISmsNotification message, UserSettings settings, NotificationSendingResult result)
        {
            try
            {
                await _sender.SendAsync(message);
            }
            catch (SmsSenderException ex)
            {
                Logger.LogError(ex, ExceptionWhileSendingSmsMessage);
                result.Errors.Add(UnspecifiedErrorMessage);
            }
        }

        protected override bool ShouldSend(NotificationEvent @event, UserSettings settings, NotificationSendingResult result)
        {
            if (string.IsNullOrWhiteSpace(settings.PhoneNumber))
            {
                result.Errors.Add(PhoneNumberIsNotConfiguredErrorMessage);
                return false;
            }

            return base.ShouldSend(@event, settings, result);
        }
    }
}
