// Developed by Softeq Development Corporation
// http://www.softeq.com

using Newtonsoft.Json;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Services.PushNotifications.Attributes;
using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Messages
{
    public class CommentLikedPushMessage : PushNotificationMessage
    {
        public CommentLikedPushMessage()
        {
            BodyLocalizationKey = "comment_Liked_body";
            TitleLocalizationKey = "comment_Liked_title";
            NotificationType = (int)NotificationEvent.CommentLiked;
        }

        [JsonProperty("userIdWhoLikedComment")]
        public string UserIdWhoLikedComment { get; set; }

        [JsonIgnore]
        [LocalizationParameter(LocalizationTarget.Title, 1)]
        [LocalizationParameter(LocalizationTarget.Body, 1)]
        public string UserNameWhoLikedComment { get; set; }

        protected override string FormatBody()
        {
            return string.IsNullOrWhiteSpace(Body) 
                ? null 
                : string.Format(Body, UserNameWhoLikedComment);
        }
    }
}
