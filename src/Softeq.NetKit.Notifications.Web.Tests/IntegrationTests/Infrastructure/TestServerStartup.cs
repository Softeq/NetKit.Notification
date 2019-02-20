// Developed by Softeq Development Corporation
// http://www.softeq.com

using GST.Fake.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Softeq.NetKit.Notifications.Web.Tests.IntegrationTests.Infrastructure
{
    public class TestServerStartup : Startup
    {
        protected override void ConfigureAdditionalServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = FakeJwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = FakeJwtBearerDefaults.AuthenticationScheme;
            }).AddFakeJwtBearer();
        }

        protected override void ConfigureAdditionalMiddleware(IApplicationBuilder app)
        {
        }

        public TestServerStartup(IConfiguration configuration, IHostingEnvironment hostingEnvironment) : base(configuration, hostingEnvironment)
        {
        }
    }
}
