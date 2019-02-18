// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Services.PushNotifications.Models;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Push
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PushNotificationSender>().AsImplementedInterfaces();
            builder.RegisterType<PushMessageFactory>().As<IMessageFactory<PushNotificationMessage>>();
            builder.RegisterType<PushMessageResourceProvider>().As<IPushMessageResourceProvider>().SingleInstance();
        }
    }
}
