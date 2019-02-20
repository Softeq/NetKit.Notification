// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Web.Tests.IntegrationTests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Notification.Response;
using Xunit;

namespace Softeq.NetKit.Notifications.Web.Tests.IntegrationTests
{
    [Collection("Store collection")]
    public class NotificationHistoryTests : IntegrationTestBase
    {
        private readonly string _userId = Guid.NewGuid().ToString();

        public NotificationHistoryTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task ListHistoryTests()
        {
            var client = GetAuthorizedUserClient(_userId);

            var response = await client.GetAsync("api/notifications/history");
            var content = await response.Content.ReadAsAsync<NotificationSetResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Empty(content.Results);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task FindHistoryTests()
        {
            var client = GetAuthorizedUserClient(_userId);

            await EnsureSettingsExist(_userId);

            var message = new SendNotificationRequest
            {
                RecipientUserId = _userId,
                EventType = NotificationEvent.ArticleCreated,
                Parameters = new Dictionary<string, object> {{"ArticleId", Guid.NewGuid()}}
            };

            var payload = new ObjectContent<SendNotificationRequest>(message, new JsonMediaTypeFormatter(), "application/json");

            var response = await client.PostAsync("api/notifications", payload);

            var content = await response.Content.ReadAsAsync<SendNotificationResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content.NotificationRecordId);

            response = await client.GetAsync($"api/notifications/history/{content.NotificationRecordId}");
            var recordContent = await response.Content.ReadAsAsync<NotificationResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(message.EventType, recordContent.Event);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task DeleteHistoryTest()
        {
            var client = GetAuthorizedUserClient(_userId);

            await EnsureSettingsExist(_userId);

            var message = new SendNotificationRequest
            {
                RecipientUserId = _userId,
                EventType = NotificationEvent.ArticleCreated,
                Parameters = new Dictionary<string, object> { { "ArticleId", Guid.NewGuid() } }
            };

            var payload = new ObjectContent<SendNotificationRequest>(message, new JsonMediaTypeFormatter(), "application/json");

            var response = await client.PostAsync("api/notifications", payload);

            var content = await response.Content.ReadAsAsync<SendNotificationResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(content.NotificationRecordId);

            response = await client.DeleteAsync("api/notifications/history");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
