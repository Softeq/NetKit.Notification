// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Softeq.NetKit.Notifications.Domain.Models.Notification;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Models
{
    internal class NotificationRecord
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public Guid UserSettingsId { get; set; }
        public DateTimeOffset Created { get; set; }
        public string OwnerUserId { get; set; }
        public NotificationEvent Event { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
