// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Services.PushNotifications.Abstractions;
using Softeq.NetKit.Services.PushNotifications.Client;

namespace Softeq.NetKit.Notifications.Web.DI
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            ConfigurePushNotificationService(builder);

            builder.RegisterModule(new Services.EmailNotifications.ContainerModule());
            //builder.RegisterModule(new Store.CosmosDB.ContainerModule());
            builder.RegisterModule(new Store.Sql.ContainerModule());
            builder.RegisterModule(new Service.ContainerModule());
        }

        private static void ConfigurePushNotificationService(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var config = context.Resolve<IConfiguration>();
                return new AzureNotificationHubConfiguration(
                    config["Notifications:Push:NotificationHub:ConnectionString"],
                    config["Notifications:Push:NotificationHub:HubName"]);
            }).SingleInstance();
            builder.RegisterType<AzureNotificationHubSender>().As<IPushNotificationSender>();
            builder.RegisterType<AzureNotificationHubSubscriber>().As<IPushNotificationSubscriber>();
        }
    }
}
