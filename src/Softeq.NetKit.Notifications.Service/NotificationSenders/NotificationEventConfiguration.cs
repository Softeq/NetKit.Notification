// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Linq;
using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders
{
    public class NotificationEventConfiguration
    {
        public static Dictionary<NotificationType, IList<NotificationEventConfiguration>> Config = new Dictionary<NotificationType, IList<NotificationEventConfiguration>>
        {
            {
                NotificationType.Email, new List<NotificationEventConfiguration>
                {
                    new NotificationEventConfiguration(NotificationEvent.ResetPassword, true),
                    new NotificationEventConfiguration(NotificationEvent.PackageArrived)
                }
            },
            {
                NotificationType.SMS, new List<NotificationEventConfiguration>
                {
                    new NotificationEventConfiguration(NotificationEvent.SendSmsCode)
                }
            },
            {
                NotificationType.Push, new List<NotificationEventConfiguration>
                {
                    new NotificationEventConfiguration(NotificationEvent.ArticleCreated),
                    new NotificationEventConfiguration(NotificationEvent.CommentLiked)
                }
            }
        };

        private NotificationEventConfiguration(NotificationEvent @event, bool isMandatory = false, bool isEnabledByDefault = true)
        {
            Event = @event;
            IsMandatory = isMandatory;
            IsEnabledByDefault = isEnabledByDefault;
        }

        public NotificationEvent Event { get; }
        public bool IsMandatory { get; }
        public bool IsEnabledByDefault { get; }

        public static NotificationEventConfiguration Find(NotificationType type, NotificationEvent @event)
        {
            var typeEvents = Config[type];
            return typeEvents.FirstOrDefault(configuration => configuration.Event == @event);
        }

        public static bool IsMandatoryEvent(NotificationType type, NotificationEvent @event)
        {
            return Find(type, @event)?.IsMandatory ?? false;
        }
    }
}
