using System.IO;
using Microsoft.Extensions.Configuration;

namespace Softeq.NetKit.Services.SmsNotifications.Tests
{
    public class SmsNotificationServiceTestConfiguration
    {
        public TwilioSmsConfiguration Configuration { get; set; }
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
        }
    }
}
