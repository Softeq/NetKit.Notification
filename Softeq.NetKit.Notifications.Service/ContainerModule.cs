// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Notifications.Service.Abstract;
using Softeq.NetKit.Notifications.Service.NotificationSenders;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Notifications.Service.Services;

namespace Softeq.NetKit.Notifications.Service
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SettingsService>().As<ISettingsService>();
            builder.RegisterType<NotificationHistoryService>().As<INotificationHistoryService>();
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<PushNotificationSubscriptionService>().As<IPushNotificationSubscriptionService>();

            builder.RegisterType<MessageValidatorProvider>().As<IMessageValidatorProvider>().SingleInstance();

            builder.RegisterModule(new NotificationSenders.Email.ContainerModule());
            builder.RegisterModule(new NotificationSenders.Push.ContainerModule());
            builder.RegisterModule(new EventBus.ContainerModule());
            builder.RegisterModule(new Services.Caching.ContainerModule());
        }
    }
}
