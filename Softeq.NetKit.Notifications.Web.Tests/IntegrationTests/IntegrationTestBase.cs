// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Net;
using Softeq.NetKit.Notifications.Web.Tests.IntegrationTests.Infrastructure;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GST.Fake.Authentication.JwtBearer;
using Softeq.NetKit.Notifications.Domain.Models.Localization;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response;
using Xunit;

namespace Softeq.NetKit.Notifications.Web.Tests.IntegrationTests
{
    public class IntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected const string DefaultUser = "Alex";
        private readonly CustomWebApplicationFactory _factory;

        public IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public HttpClient GetUnauthorizedUserClient()
        {
            return _factory.CreateClient();
        }

        public HttpClient GetAuthorizedUserClient(string userId = DefaultUser)
        {
            var client = _factory.CreateClient().SetFakeBearerToken(userId);

            client.BaseAddress=new Uri("https://localhost");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        protected readonly UserProfileRequest DefaultProfile = new UserProfileRequest
        {
            Email = "alex@",
            FirstName = "alex",
            Language = LanguageName.Fr,
            LastName = "softeq",
            PhoneNumber = "123"
        };

        protected async Task EnsureSettingsExist(string userId)
        {
            var client = GetAuthorizedUserClient(userId);

            var response = await client.GetAsync("api/settings");
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                response = await client.PostAsync("api/settings", new ObjectContent<UserProfileRequest>(DefaultProfile, new JsonMediaTypeFormatter()));
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            }
            else
            {
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }

            var content = await response.Content.ReadAsAsync<UserProfileResponse>();
            Assert.Equal(DefaultProfile.Email, content.Email);
            Assert.Equal(DefaultProfile.FirstName, content.FirstName);
            Assert.Equal(DefaultProfile.Language, content.Language);
            Assert.Equal(DefaultProfile.LastName, content.LastName);
            Assert.Equal(DefaultProfile.PhoneNumber, content.PhoneNumber);
        }
    }
}
