// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.Models.Localization;
using Softeq.NetKit.Notifications.Domain.Models.Notification;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Request;
using Softeq.NetKit.Notifications.Service.TransportModels.Settings.Response;
using Softeq.NetKit.Notifications.Web.Tests.IntegrationTests.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Notifications.Web.Tests.IntegrationTests
{
    [Collection("Store collection")]
    public class SettingsTests : IntegrationTestBase
    {
        private readonly string _userId = Guid.NewGuid().ToString();

        public SettingsTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task CreateSettingsTest()
        {
            var userId = Guid.NewGuid().ToString();
            var client = GetAuthorizedUserClient(userId);
            var payload = new ObjectContent<UserProfileRequest>(DefaultProfile, new JsonMediaTypeFormatter(), "application/json");
            var response = await client.PostAsync("api/settings", payload);
            var content = await response.Content.ReadAsAsync<UserSettingsResponse>();

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(DefaultProfile.Email, content.Email);
            Assert.Equal(DefaultProfile.FirstName, content.FirstName);
            Assert.Equal(DefaultProfile.Language, content.Language);
            Assert.Equal(DefaultProfile.LastName, content.LastName);
            Assert.Equal(DefaultProfile.PhoneNumber, content.PhoneNumber);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetSettingsTest()
        {
            var client = GetAuthorizedUserClient(_userId);

            await EnsureSettingsExist(_userId);

            var response = await client.GetAsync("api/settings");
            var content = await response.Content.ReadAsAsync<UserProfileResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(_userId, content.UserId);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UpdateSettingsTest()
        {
            var client = GetAuthorizedUserClient(_userId);

            await EnsureSettingsExist(_userId);

            var updatedProfile = new UserProfileRequest
            {
                Email = "ben",
                FirstName = "alex",
                Language = LanguageName.Ru,
                LastName = "ms",
                PhoneNumber = "456"
            };

            var payload = new ObjectContent<UserProfileRequest>(updatedProfile, new JsonMediaTypeFormatter(), "application/json");
            var response = await client.PutAsync("api/settings", payload);
            var content = await response.Content.ReadAsAsync<UserProfileResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(updatedProfile.Email, content.Email);
            Assert.Equal(updatedProfile.FirstName, content.FirstName);
            Assert.Equal(updatedProfile.Language, content.Language);
            Assert.Equal(updatedProfile.LastName, content.LastName);
            Assert.Equal(updatedProfile.PhoneNumber, content.PhoneNumber);
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task GetNotificationSettingsTest()
        {
            var client = GetAuthorizedUserClient(_userId);
            await EnsureSettingsExist(_userId);

            var response = await client.GetAsync("api/settings/notifications");
            var content = await response.Content.ReadAsAsync<NotificationSettingsResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(4, content.Settings.Count());
            Assert.True(content.Settings.All(x => x.Enabled));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task UpdateNotificationSettingsTest()
        {
            var client = GetAuthorizedUserClient(_userId);
            await EnsureSettingsExist(_userId);

            var request = new UpdateNotificationSettingsRequest
            {
                Settings = new List<NotificationSettingModel>
                {
                    new NotificationSettingModel
                    {
                        Type = NotificationType.Email,
                        Event = NotificationEvent.PackageArrived,
                        Enabled = false
                    }
                }
            };

            var payload = new ObjectContent<UpdateNotificationSettingsRequest>(request, new JsonMediaTypeFormatter(), "application/json");
            var response = await client.PutAsync("api/settings/notifications", payload);
            var content = await response.Content.ReadAsAsync<NotificationSettingsResponse>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(4, content.Settings.Count());
            Assert.False(content.Settings.First(x => x.Type == NotificationType.Email && x.Event == NotificationEvent.PackageArrived).Enabled);
            Assert.True(content.Settings.Where(x => x.Type != NotificationType.Email && x.Event != NotificationEvent.PackageArrived).All(x => x.Enabled));
        }
    }
}
