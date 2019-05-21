﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using Autofac;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Services.SmsNotifications.Abstract;
using Softeq.NetKit.Services.SmsNotifications.SmsSender;

namespace Softeq.NetKit.Services.SmsNotifications
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context =>
            {
                var config = context.Resolve<IConfiguration>();
                return new TwilioSmsConfiguration()
                {
                    AccountSid = config["Notifications:Sms:Twilio:AccountSid"],
                    AuthToken = config["Notifications:Sms:Twilio:AuthToken"],
                    FromNumber = config["Notifications:Sms:Twilio:FromNumber"]
                };
            }).SingleInstance();
            builder.RegisterType<TwilioSmsSender>().As<ISmsSender>();
            builder.RegisterType<SmsNotificationService>().As<ISmsNotificationService>();
        }
    }
}
