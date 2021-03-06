﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Logging;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Services.EmailNotifications.Abstract;
using Softeq.NetKit.Services.EmailNotifications.Exception;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email
{
    internal class EmailNotificationSender : BaseNotificationSender<IEmailNotification>
    {
        private readonly IEmailNotificationService _sender;

        public EmailNotificationSender(IEmailNotificationService sender,
            IMessageFactory<IEmailNotification> messageFactory,
            IMessageValidatorProvider validatorProvider,
            ILoggerFactory loggerFactory) : base(messageFactory, validatorProvider, loggerFactory)
        {
            _sender = sender;
        }

        protected override async Task PerformSendingAsync(IEmailNotification message, UserSettings settings, NotificationSendingResult result)
        {
            try
            {
                await _sender.SendAsync(message);
            }
            catch (EmailSenderException ex)
            {
                Logger.LogError(ex, "Exception while sending email notification");
                result.Errors.Add("Unspecified error");
            }
        }

        protected override NotificationType SenderType => NotificationType.Email;

        protected override bool ShouldSend(NotificationEvent @event, UserSettings settings, NotificationSendingResult result)
        {
            if (string.IsNullOrWhiteSpace(settings.Email))
            {
                result.Errors.Add("User email address is not configured");
                return false;
            }

            return base.ShouldSend(@event, settings, result);
        }
    }
}
