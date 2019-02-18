// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Store.Sql.Mappings.Abstract;
using Softeq.NetKit.Notifications.Store.Sql.Models;

namespace Softeq.NetKit.Notifications.Store.Sql.Mappings
{
    internal class NotificationHistoryMapping : EntityMappingConfiguration<NotificationRecord>
    {
        public override void Map(EntityTypeBuilder<NotificationRecord> builder)
        {
            builder.HasKey(entity => entity.Id);
            builder.HasOne(p => p.UserSettings)
                .WithMany(x => x.NotificationRecords)
                .HasForeignKey(record => record.UserSettingsId);
            builder.Property(record => record.Parameters)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    v => JsonConvert.DeserializeObject<Dictionary<string, object>>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            builder.Property(x=>x.Event)
                .HasConversion(
                    v => v.ToString(),
                    v => (NotificationEvent)Enum.Parse(typeof(NotificationEvent), v));
        }
    }
}
