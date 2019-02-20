// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Newtonsoft.Json;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push.Messages
{
    public class ArticleCreatedPushMessage : PushNotificationMessage
    {
        [JsonProperty("articleId")]
        public Guid ArticleId { get; set; }

        public ArticleCreatedPushMessage()
        {
            NotificationType = (int)NotificationEvent.ArticleCreated;
            BodyLocalizationKey = "article_NewCreated_body";
            TitleLocalizationKey = "article_NewCreated_title";
        }
    }
}
