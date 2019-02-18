// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.IO;
using Softeq.NetKit.Notifications.Web.Infrastructure.Logging;

namespace Softeq.NetKit.Notifications.Web.Tests.IntegrationTests.Infrastructure
{
    public class CustomWebApplicationFactory : WebApplicationFactory<TestServerStartup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return WebHost.CreateDefaultBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json");

                    config.AddEnvironmentVariables();
                })
                .UseStartup<TestServerStartup>()
                .UseSerilog();
        }
    }
}
