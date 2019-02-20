// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Logging;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Services.PushNotifications.Abstractions;
using Softeq.NetKit.Services.PushNotifications.Exception;
using Softeq.NetKit.Services.PushNotifications.Models;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push
{
    internal class PushNotificationSender : BaseNotificationSender<PushNotificationMessage>
    {
        private readonly IPushNotificationSender _sender;

        public PushNotificationSender(IPushNotificationSender sender,
            IMessageFactory<PushNotificationMessage> factory,
            IMessageValidatorProvider validatorProvider,
            ILoggerFactory loggerFactory) : base(factory, validatorProvider, loggerFactory)
        {
            _sender = sender;
        }

        protected override NotificationType SenderType => NotificationType.Push;

        protected override async Task PerformSending(PushNotificationMessage message, UserSettings settings, NotificationSendingResult result)
        {
            try
            {
                var isSent = await _sender.SendAsync(message, TagHelper.GetUserTag(settings.UserId));
                if (!isSent)
                {
                    Logger.LogWarning("Failed to send push notification");
                    result.Errors.Add("Unspecified error");
                }
            }
            catch (ValidationException ex)
            {
                Logger.LogError(ex, "Exception while validating push message");
                result.Errors.Add("Validation Error");
            }
            catch (PushNotificationException ex)
            {
                Logger.LogError(ex, "Exception while sending push notification");
                result.Errors.Add("Unspecified error");
            }
        }
    }
}
