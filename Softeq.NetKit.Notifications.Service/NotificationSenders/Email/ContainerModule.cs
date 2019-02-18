// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Softeq.NetKit.Notifications.Service.NotificationSenders.Abstractions;
using Softeq.NetKit.Services.EmailNotifications.Abstract;

namespace Softeq.NetKit.Notifications.Service.NotificationSenders.Email
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmailNotificationSender>().AsImplementedInterfaces();
            builder.RegisterType<EmailMessageFactory>().As<IMessageFactory<IEmailNotification>>();
            builder.RegisterType<EmailMessageResourceProvider>().As<IEmailMessageResourceProvider>().SingleInstance();
        }
    }
}
