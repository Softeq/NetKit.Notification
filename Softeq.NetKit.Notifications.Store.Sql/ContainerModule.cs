// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Infrastructure;
using Softeq.NetKit.Notifications.Store.Sql.DataStores;
using Softeq.NetKit.Notifications.Store.Sql.Setup;

namespace Softeq.NetKit.Notifications.Store.Sql
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var config = context.Resolve<IConfiguration>();
                return new StorageConfiguration
                {
                    ConnectionString = config["Notifications:Storage:Sql:ConnectionString"]
                };
            }).SingleInstance();

            builder.Register(context =>
            {
                var config = context.Resolve<StorageConfiguration>();
                var dbContextOptionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlServer(config.ConnectionString, optionsBuilder => optionsBuilder.EnableRetryOnFailure());
                return dbContextOptionsBuilder.Options;
            }).SingleInstance();

            builder.RegisterType<ApplicationDbContext>().InstancePerLifetimeScope();
            builder.RegisterType<DatabaseBootstrapper>().As<IBootstrapper>();

            builder.RegisterType<SettingsDataStore>()
                .As<ISettingsDataStore>()
                .Named<ISettingsDataStore>(typeof(ISettingsDataStore).Name);

            builder.RegisterType<NotificationRecordDataStore>()
                .As<INotificationHistoryDataStore>()
                .Named<INotificationHistoryDataStore>(typeof(INotificationHistoryDataStore).Name);
        }
    }
}
