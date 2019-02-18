// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Softeq.NetKit.Notifications.Domain.Models.Localization;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Models
{
    internal class UserSettings 
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public LanguageName Language { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public IList<NotificationSetting> Settings { get; set; }
    }
}
