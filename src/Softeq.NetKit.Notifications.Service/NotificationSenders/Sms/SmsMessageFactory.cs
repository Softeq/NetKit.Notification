// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.Extensions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Sms.Messages;
using Softeq.NetKit.Services.SmsNotifications.Abstract;
using System;
using System.Collections.Generic;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Sms
{
    internal class SmsMessageFactory : IMessageFactory<ISmsNotification>
    {
        private const string EventIsNotSupportedExceptionMessage = "{0} event is not supported";

        private static readonly Dictionary<NotificationEvent, Type> registry = new Dictionary<NotificationEvent, Type>
        {
            {NotificationEvent.SmsSent, typeof(SmsMessage)}
        };

        public ISmsNotification Create(NotificationMessage message, UserProfileSettings settings)
        {
            if (!registry.TryGetValue(message.Event, out var messageType))
            {
                throw new InvalidOperationException(string.Format(EventIsNotSupportedExceptionMessage, message.Event));
            }

            var notification = (ISmsNotification)DynamicExtensions.ToStatic(messageType, message.Parameters);

            return notification;
        }
    }
}
