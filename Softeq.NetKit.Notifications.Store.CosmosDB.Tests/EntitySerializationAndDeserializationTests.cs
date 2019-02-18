// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Models;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using Softeq.NetKit.Notifications.Store.CosmosDB.Tests.Utility;
using Xunit;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Tests
{
    public class EntitySerializationAndDeserializationTests : BaseNotificationClientFixture
    {
        private static int pageSize = 50;
        public EntitySerializationAndDeserializationTests()
        {
            DatabaseCleaner.CleanupTestDatabaseAsync().GetAwaiter();
        }

        [Fact]
        public async Task ShouldReturnEmptyList()
        {
            var ownerId = "F8F98F91-339D-4453-87F3-AEC05AD976DD";
            var fetchedNotifications = await Store.ListAsync(ownerId, pageSize, new FilterOptions());
            Assert.Empty(fetchedNotifications);
        }

        [Fact]
        public async Task ShouldPostAndGetTemplateLikedNotification()
        {
            var ownerId = "F8F98F91-339D-4453-87F3-AEC05AD976EE";

            var notification = new NotificationRecord
            {
                OwnerUserId = ownerId,
                Parameters = new Dictionary<string, object>
                {
                    {"UserIdWhoLiked", "14CFB9BC-9055-4E0B-A547-2F1A9208B45A" }
                }
            };
            await Store.SaveAsync(notification);
            var fetchedNotifications = await Store.ListAsync(ownerId, pageSize, new FilterOptions());

            Assert.True(fetchedNotifications.Count() == 1);
            var fetchedNotification = fetchedNotifications.First();
            ValidateFetchedNotification(notification, fetchedNotification);
        }

        private void ValidateFetchedNotification(NotificationRecord expectedNotification, NotificationRecord fetchedNotification)
        {
            Assert.NotNull(expectedNotification);
            Assert.NotNull(fetchedNotification);
            Assert.True(fetchedNotification.Id != null);
            Assert.Same(expectedNotification, fetchedNotification);
        }
    }
}
