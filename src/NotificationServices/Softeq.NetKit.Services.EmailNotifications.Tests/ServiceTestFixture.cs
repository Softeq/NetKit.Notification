// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Configuration;

namespace Softeq.NetKit.Services.EmailNotifications.Tests
{
    public class ServiceTestFixture
    {
        public SendGridClientConfiguration Configuration { get; }

        public ServiceTestFixture()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            Configuration = new SendGridClientConfiguration
            {
                ApiKey = configuration["Notifications:Mail:SendGrid:ApiKey"],
                FromName = configuration["Notifications:Mail:SendGrid:FromName"],
                FromEmail = configuration["Notifications:Mail:SendGrid:FromEmail"]
            };
        }
    }
}
