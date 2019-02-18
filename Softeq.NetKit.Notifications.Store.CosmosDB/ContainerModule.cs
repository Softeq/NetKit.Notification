// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Infrastructure;
using Softeq.NetKit.Notifications.Store.CosmosDB.Client;
using Softeq.NetKit.Notifications.Store.CosmosDB.DataStores;
using Softeq.NetKit.Notifications.Store.CosmosDB.Setup;
using Module = Autofac.Module;

namespace Softeq.NetKit.Notifications.Store.CosmosDB
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CosmosDbClientFactory>().As<ICosmosDbClientFactory>();

            builder.Register(context =>
            {
                var config = context.Resolve<IConfiguration>();
                return new StorageConfiguration
                {
                    NotificationTtlDays = int.Parse(config["Notifications:Storage:CosmosDB:NotificationTtlDays"]),
                    DatabaseId = config["Notifications:Storage:CosmosDB:DatabaseId"],
                    BulkDeleteNotificationStoredProcedureId = config["Notifications:Storage:CosmosDB:BulkDeleteNotificationStoredProcedureId"],
                    DefaultRUs = int.Parse(config["Notifications:Storage:CosmosDB:DefaultRUs"]),
                    Endpoint = new Uri(config["Notifications:Storage:CosmosDB:Endpoint"]),
                    Key = config["Notifications:Storage:CosmosDB:Key"]
                };
            }).SingleInstance();

            builder.RegisterType<StorageBootstrapper>().As<IBootstrapper>();

            builder.RegisterType<SettingsDataStore>()
                .As<ISettingsDataStore>()
                .Named<ISettingsDataStore>(typeof(ISettingsDataStore).Name);

            builder.RegisterType<NotificationRecordDataStore>()
                .As<INotificationHistoryDataStore>()
                .Named<INotificationHistoryDataStore>(typeof(INotificationHistoryDataStore).Name);
        }
    }
}
