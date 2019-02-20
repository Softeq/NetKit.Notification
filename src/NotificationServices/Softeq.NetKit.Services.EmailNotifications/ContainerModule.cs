// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Services.EmailNotifications.Abstract;
using Softeq.NetKit.Services.EmailNotifications.EmailSender;

namespace Softeq.NetKit.Services.EmailNotifications
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var config = context.Resolve<IConfiguration>();
                return new SendGridClientConfiguration
                {
                    ApiKey = config["Notifications:Mail:SendGrid:ApiKey"],
                    FromName = config["Notifications:Mail:SendGrid:FromName"],
                    FromEmail = config["Notifications:Mail:SendGrid:FromEmail"]
                };
            }).SingleInstance();
            builder.RegisterType<SendGridEmailSender>().As<IEmailSender>();
            builder.RegisterType<EmailNotificationService>().As<IEmailNotificationService>();
        }
    }
}
