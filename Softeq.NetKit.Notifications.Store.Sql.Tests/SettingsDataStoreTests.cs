// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Domain.Models.Localization;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Store.Sql.DataStores;
using Xunit;

namespace Softeq.NetKit.Notifications.Store.Sql.Tests
{
    public class SettingsDataStoreTests : UnitTestBase
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task SaveSettingsTest()
        {
            var newRecord = new UserSettings
            {
                Email = "alex@softeq.com",
                FirstName = "Alex",
                Language = LanguageName.Fr,
                LastName = "Softeq",
                PhoneNumber = "123",
                UserId = Guid.NewGuid().ToString()
            };

            using (var context = GetContext())
            {
                var store = new SettingsDataStore(context, DefaultMapper);
                var result = await store.SaveAsync(newRecord);
                Assert.NotEqual(Guid.Empty, result.Id);
                Assert.NotEqual(new DateTimeOffset(), result.Created);
                Assert.NotEqual(new DateTimeOffset(), result.Updated);
            }

            using (var context = GetContext())
            {
                Assert.NotEmpty(context.UserSettings);
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task DoesExistAsyncTest()
        {
            var newRecord = new Models.UserSettings
            {
                Email = "alex@softeq.com",
                FirstName = "Alex",
                Language = LanguageName.Fr,
                LastName = "Softeq",
                PhoneNumber = "123",
                UserId = Guid.NewGuid().ToString(),
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid()
            };

            using (var context = GetContext())
            {
                await context.UserSettings.AddAsync(newRecord);
                await context.SaveChangesAsync();
            }

            using (var context = GetContext())
            {
                var store = new SettingsDataStore(context, DefaultMapper);
                var result = await store.DoesExistAsync(newRecord.UserId);
                Assert.True(result);
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task FindAsyncTest()
        {
            var newRecord = new Models.UserSettings
            {
                Email = "alex@softeq.com",
                FirstName = "Alex",
                Language = LanguageName.Fr,
                LastName = "Softeq",
                PhoneNumber = "123",
                UserId = Guid.NewGuid().ToString(),
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid()
            };

            using (var context = GetContext())
            {
                await context.UserSettings.AddAsync(newRecord);
                await context.SaveChangesAsync();
            }

            using (var context = GetContext())
            {
                var store = new SettingsDataStore(context, DefaultMapper);
                var result = await store.FindAsync(newRecord.UserId);
                Assert.NotNull(result);
                Assert.Equal(newRecord.Email, result.Email);
                Assert.Equal(newRecord.FirstName, result.FirstName);
                Assert.Equal(newRecord.Language, result.Language);
                Assert.Equal(newRecord.LastName, result.LastName);
                Assert.Equal(newRecord.PhoneNumber, result.PhoneNumber);
                Assert.Equal(newRecord.Id, result.Id);
                Assert.Equal(newRecord.Created, result.Created);
                Assert.Equal(newRecord.Updated, result.Updated);
            }
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task UpdateAsyncTest()
        {
            var user = new Models.UserSettings
            {
                Email = "alex@softeq.com",
                FirstName = "Alex",
                Language = LanguageName.Fr,
                LastName = "Softeq",
                PhoneNumber = "123",
                UserId = Guid.NewGuid().ToString(),
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid()
            };

            using (var context = GetContext())
            {
                await context.UserSettings.AddAsync(user);
                await context.SaveChangesAsync();
            }

            var updates = new UserSettings
            {
                Email = "ben@softeq.com",
                FirstName = "ben",
                Language = LanguageName.Ru,
                LastName = "ms",
                PhoneNumber = "456",
                UserId = Guid.NewGuid().ToString(),
                Id = user.Id,
                Created = user.Created
            };

            using (var context = GetContext())
            {
                var store = new SettingsDataStore(context, DefaultMapper);
                var result = await store.UpdateAsync(updates);
                Assert.NotNull(result);
                Assert.Equal(updates.Email, result.Email);
                Assert.Equal(updates.FirstName, result.FirstName);
                Assert.Equal(updates.Language, result.Language);
                Assert.Equal(updates.LastName, result.LastName);
                Assert.Equal(updates.PhoneNumber, result.PhoneNumber);
                Assert.Equal(updates.Id, result.Id);
                Assert.Equal(user.Created, result.Created);
                Assert.True(user.Updated < result.Updated);
            }
        }
    }
}
