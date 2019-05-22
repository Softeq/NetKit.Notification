// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders
{
    internal abstract class BaseNotificationSender<TMessage> : INotificationSender
    {
        protected ILogger Logger;
        private readonly IMessageFactory<TMessage> _messageFactory;
        private readonly IMessageValidatorProvider _validatorProvider;

        protected BaseNotificationSender(IMessageFactory<TMessage> messageFactory, IMessageValidatorProvider validatorProvider, ILoggerFactory loggerFactory)
        {
            _messageFactory = messageFactory;
            _validatorProvider = validatorProvider;
            Logger = loggerFactory.CreateLogger(GetType().Name);
        }

        protected abstract NotificationType SenderType { get; }

        public async Task<NotificationSendingResult> SendAsync(NotificationMessage message, UserSettings settings)
        {
            var result = new NotificationSendingResult(SenderType);

            if (!ShouldSend(message.Event, settings, result))
            {
                return result;
            }

            var notification = _messageFactory.Create(message, settings);

            var validator = _validatorProvider.GetValidator(notification);
            var validationResult = await validator.ValidateAsync(notification);
            if (!validationResult.IsValid)
            {
                Logger.LogWarning($"Invalid {SenderType} notification message state", validationResult.Errors);
                result.Errors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));
                return result;
            }

            await PerformSendingAsync(notification, settings, result);

            return result;
        }

        protected virtual bool ShouldSend(NotificationEvent @event, UserSettings settings, NotificationSendingResult result)
        {
            var configuration = NotificationEventConfiguration.Find(SenderType, @event);
            if (configuration == null)
            {
                result.Skip();
                return false;
            }

            if (configuration.IsMandatory)
            {
                return true;
            }

            var eventSetting = settings.Settings.FirstOrDefault(s => s.Type == SenderType && s.Event == @event);
            var isEnabled = eventSetting?.Enabled ?? configuration.IsEnabledByDefault;
            if (!isEnabled)
            {
                result.Skip();
            }

            return isEnabled;
        }

        protected abstract Task PerformSendingAsync(TMessage message, UserSettings settings, NotificationSendingResult result);
    }
}
