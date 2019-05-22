﻿// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Softeq.NetKit.Services.SmsNotifications.SmsSender;
using Twilio.Clients;

namespace Softeq.NetKit.Services.SmsNotifications.Tests
{
    public class SmsNotificationServiceTestConfiguration
    {
        public TwilioSmsConfiguration Configuration { get; set; }
        public string RecipientPhoneNumber { get; set; }
        public readonly ITwilioRestClient TwilioRestClient;

        public SmsNotificationServiceTestConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var config = builder.Build();
            Configuration = new TwilioSmsConfiguration
            {
                AccountSid = config["Notifications:Sms:Twilio:AccountSid"],
                AuthToken = config["Notifications:Sms:Twilio:AuthToken"],
                FromNumber = config["Notifications:Sms:Twilio:FromNumber"]
            };
            RecipientPhoneNumber = config["Notifications:Sms:Twilio:RecipientPhoneNumber"];
            TwilioRestClient = new TwilioHttpClient(new HttpClient(), Configuration);
        }
    }
}
