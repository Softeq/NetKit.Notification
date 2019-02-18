// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.Localization;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.Extensions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Email.Messages;
using Softeq.NetKit.Services.EmailNotifications.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email
{
    internal class EmailMessageFactory : IMessageFactory<IEmailNotification>
    {
        private readonly IEmailMessageResourceProvider _resourceProvider;

        private static readonly Dictionary<NotificationEvent, Type> registry = new Dictionary<NotificationEvent, Type>
        {
            {NotificationEvent.PackageArrived, typeof(PackageArrivedEmailMessage)},
            {NotificationEvent.ResetPassword, typeof(ForgotPasswordEmailMessage)}
        };

        public EmailMessageFactory(IEmailMessageResourceProvider resourceProvider)
        {
            _resourceProvider = resourceProvider;
        }

        public IEmailNotification Create(NotificationMessage message, UserProfileSettings settings)
        {
            if (!registry.TryGetValue(message.Event, out var messageType))
            {
                throw new InvalidOperationException($"{message.Event} event is not supported");
            }

            var modelType = messageType.BaseType.GetGenericArguments().First();
            var model = DynamicExtensions.ToStatic(modelType, message.Parameters);
            var notification = (IEmailNotification)Activator.CreateInstance(messageType, settings.Email, settings.FullName, model);

            SetContent(modelType.Name, settings.Language, notification);

            return notification;
        }

        private void SetContent(string messageName, LanguageName language, IEmailNotification message)
        {
            message.BaseHtmlTemplate = _resourceProvider.GetBaseTemplate();
            message.HtmlTemplate = _resourceProvider.GetMainTemplate(messageName, language);

            message.Subject = _resourceProvider.GetSubjectString(messageName, language);
        }
    }
}
