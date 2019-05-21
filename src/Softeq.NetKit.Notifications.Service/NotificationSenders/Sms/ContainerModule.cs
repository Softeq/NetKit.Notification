// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Services.SmsNotifications.Abstract;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Sms
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SmsNotificationSender>().AsImplementedInterfaces();
            builder.RegisterType<SmsMessageFactory>().As<IMessageFactory<ISmsNotification>>();
        }
    }
}
