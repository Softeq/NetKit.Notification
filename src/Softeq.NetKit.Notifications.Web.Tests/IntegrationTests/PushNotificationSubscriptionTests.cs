// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Models.PushNotification;
using Softeq.NetKit.Notifications.Service.TransportModels.PushNotification.Request;
using Softeq.NetKit.Notifications.Web.Tests.IntegrationTests.Infrastructure;
using Xunit;

namespace Softeq.NetKit.Notifications.Web.Tests.IntegrationTests
{
    [Collection("Store collection")]
    public class PushNotificationSubscriptionTests : IntegrationTestBase
    {
        private readonly string _userId = Guid.NewGuid().ToString();

        public PushNotificationSubscriptionTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task SubscribeDeviceTest()
        {
            var client = GetAuthorizedUserClient(_userId);

            await EnsureSettingsExist(_userId);

            var request = new PushDeviceRequest(DevicePlatform.iOS, "670fb78e33df6dc0413b31a0698a3a830000aa000ea5aae563d4212e6e5b527b");
            var payload = new ObjectContent<PushDeviceRequest>(request, new JsonMediaTypeFormatter(), "application/json");

            var response = await client.PostAsync("api/notifications/push/subscription", payload);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UnsubscribeDeviceTest()
        {
            var client = GetAuthorizedUserClient(_userId);

            await EnsureSettingsExist(_userId);

            var request = new PushDeviceRequest(DevicePlatform.iOS, "670fb78e33df6dc0413b31a0698a3a830000aa000ea5aae563d4212e6e5b527b");
            var payload = new ObjectContent<PushDeviceRequest>(request, new JsonMediaTypeFormatter(), "application/json");

            var msg = new HttpRequestMessage
            {
                Content = payload,
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"{client.BaseAddress}api/notifications/push/subscription")
            };
            var response = await client.SendAsync(msg);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UnsubscribeUserTest()
        {
            var client = GetAuthorizedUserClient(_userId);

            await EnsureSettingsExist(_userId);

            var response = await client.DeleteAsync("api/notifications/push/subscription/me");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
