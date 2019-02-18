// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Store.CosmosDB.Tests.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Models;
using Xunit;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Tests
{
    public class DocumentDbNotificationsClientTests : BaseNotificationClientFixture
    {
        private static int pageSize = 50;
        public DocumentDbNotificationsClientTests()
        {
            DatabaseCleaner.CleanupTestDatabaseAsync().GetAwaiter();
        }

        [Fact]
        public async Task ShouldRemoveNotificationAsyncTest()
        {
            var ownerId = "6E86B3CB-1EEB-44BB-A020-6991B23F924D";

            var notification = new Domain.Models.NotificationRecord.NotificationRecord
            {
                OwnerUserId = ownerId,
                Parameters = new Dictionary<string, object>
                {
                    {"UserIdWhoLiked", "14CFB9BC-9055-4E0B-A547-2F1A9208B45A" }
                }
            };

            var newRecord = await Store.SaveAsync(notification);
            var fetchedNotificationsAfterInsert = await Store.FindAsync(newRecord.Id);

            // ensure that notification was add to DB
            Assert.NotNull(fetchedNotificationsAfterInsert);

            // verify delete
            await Store.DeleteAllAsync(ownerId);
            var fetchedNotifications = await Store.ListAsync(ownerId, pageSize, new FilterOptions());
            Assert.Empty(fetchedNotifications);
        }

        [Fact]
        public async Task ShouldPostNotificationAsyncTest()
        {
            var ownerId = "9B972F1C-6577-473F-BBAF-D21C022F3DA8";
            var notification = new Domain.Models.NotificationRecord.NotificationRecord
            {
                OwnerUserId = ownerId,
                Parameters = new Dictionary<string, object>
                {
                    {"UserIdWhoLiked", "14CFB9BC-9055-4E0B-A547-2F1A9208B45A" }
                }
            };

            var notifications = await Store.ListAsync(ownerId, pageSize, new FilterOptions());
            Assert.Empty(notifications);

            await Store.SaveAsync(notification);

            var newNotifications = await Store.ListAsync(ownerId, pageSize, new FilterOptions());
            Assert.NotEmpty(newNotifications);
            Assert.True(newNotifications.Count() == 1);
        }

        [Fact]
        public async Task ShouldGetNotificationByIdAsyncTest()
        {
            var ownerId = "9B972F1C-6577-473F-BBAF-D21C022F3DA8";
            var userIdWhoLiked = "9B922F1C-6577-473F-BBAF-D21C022F3DA8";
            var notification = new Domain.Models.NotificationRecord.NotificationRecord
            {
                OwnerUserId = ownerId,
                Parameters = new Dictionary<string, object>
                {
                    {"UserIdWhoLiked",userIdWhoLiked }
                }
            };

            await Store.SaveAsync(notification);
            var newNotification = await Store.FindAsync(notification.Id);

            Assert.NotNull(newNotification);
            Assert.Equal(newNotification.OwnerUserId, notification.OwnerUserId);
            Assert.Equal(newNotification.Parameters["UserIdWhoLiked"], userIdWhoLiked);
            Assert.Equal(newNotification.Id, notification.Id);
            Assert.Equal(newNotification.Event, notification.Event);
        }

        [Fact]
        public async Task ShouldGetNotificationsAsyncTest()
        {
            var ownerId = "9B972F1C-6577-473F-BBAF-D21C022F3DA8";
            var notification = new Domain.Models.NotificationRecord.NotificationRecord
            {
                OwnerUserId = ownerId,
                Parameters = new Dictionary<string, object>
                {
                    {"UserIdWhoLiked", "14CFB9BC-9055-4E0B-A547-2F1A9208B45A" }
                }
            };

            await Store.SaveAsync(notification);
            var notifications = await Store.ListAsync(ownerId, pageSize, new FilterOptions());

            Assert.NotEmpty(notifications);
            Assert.True(notifications.Count() == 1);
        }
    }
}
