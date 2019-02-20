// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.Extensions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Messages;
using Softeq.NetKit.Services.PushNotifications.Models;
using System;
using System.Collections.Generic;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push
{
    internal class PushMessageFactory : IMessageFactory<PushNotificationMessage>
    {
        private readonly IPushMessageResourceProvider _resourceProvider;

        private static readonly Dictionary<NotificationEvent, Type> registry = new Dictionary<NotificationEvent, Type>
        {
            {NotificationEvent.ArticleCreated, typeof(ArticleCreatedPushMessage)},
            {NotificationEvent.CommentLiked, typeof(CommentLikedPushMessage)}
        };

        public PushMessageFactory(IPushMessageResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        public PushNotificationMessage Create(NotificationMessage message, UserProfileSettings settings)
        {
            if (!registry.TryGetValue(message.Event, out var messageType))
            {
                throw new InvalidOperationException($"{message.Event} event is not supported");
            }

            PushNotificationMessage notification = (PushNotificationMessage)DynamicExtensions.ToStatic(messageType, message.Parameters);
            var typeName = messageType.Name;
            notification.Body = _resourceProvider.GetBody(typeName, settings.Language);
            notification.Title = _resourceProvider.GetTitle(typeName, settings.Language);
            return notification;
        }
    }
}
