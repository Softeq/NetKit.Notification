// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Softeq.NetKit.Notifications.Domain.Models.Localization;
using Softeq.NetKit.Notifications.Store.Sql.Mappings.Abstract;
using Softeq.NetKit.Notifications.Store.Sql.Models;

namespace Softeq.NetKit.Notifications.Store.Sql.Mappings
{
    internal class UserSettingsMapping : EntityMappingConfiguration<UserSettings>
    {
        public override void Map(EntityTypeBuilder<UserSettings> builder)
        {
            builder.HasKey(entity => entity.Id);
            builder.HasMany(p => p.Settings)
                .WithOne(x => x.UserSettings)
                .HasForeignKey(b => b.UserSettingsId);

            builder.HasMany(p => p.NotificationRecords)
                .WithOne(p => p.UserSettings)
                .HasForeignKey(record => record.UserSettingsId);

            builder.Property(x => x.Language)
                .HasConversion(
                    v => v.ToString(),
                    v => (LanguageName)Enum.Parse(typeof(LanguageName), v));
        }
    }
}
