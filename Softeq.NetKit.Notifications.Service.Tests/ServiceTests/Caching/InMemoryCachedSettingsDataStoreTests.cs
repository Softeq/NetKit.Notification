// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Caching.Memory;
using Moq;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using Softeq.NetKit.Notifications.Service.Services.Caching;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Softeq.NetKit.Notifications.Service.Tests.ServiceTests.Caching
{
    public class InMemoryCachedSettingsDataStoreTests
    {
        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenSavingNewEntityObjectIsNotAddedToCacheTest()
        {
            var cache = new MemoryCache(new MemoryCacheOptions());
            var innerStore = new Mock<ISettingsDataStore>();
            UserSettings newSettings = null;
            innerStore.Setup(x => x.SaveAsync(It.IsAny<UserSettings>()))
                .Callback<UserSettings>(settings => newSettings = settings)
                .ReturnsAsync(() => newSettings);

            var store = new InMemoryCachedSettingsDataStore(innerStore.Object, cache, new InMemoryCachingOptions());
            var result = await store.SaveAsync(new UserSettings());

            Assert.NotNull(result);
            Assert.Equal(0, cache.Count);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenCheckingForExistenceAndItemPresentThenStoreNotHitTest()
        {
            var userId = Guid.NewGuid().ToString();
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.GetOrCreate(GetKey(userId), entry => new UserSettings { UserId = userId });
            var innerStore = new Mock<ISettingsDataStore>();

            var store = new InMemoryCachedSettingsDataStore(innerStore.Object, cache, new InMemoryCachingOptions());
            var result = await store.DoesExistAsync(userId);

            Assert.True(result);

            innerStore.Verify(x => x.DoesExistAsync(It.IsAny<string>()), Times.Never);
            Assert.Equal(1, cache.Count);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenCheckingForExistenceAndItemNotPresentThenStoreHitTest()
        {
            var userId = Guid.NewGuid().ToString();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var innerStore = new Mock<ISettingsDataStore>();
            innerStore.Setup(x => x.DoesExistAsync(It.Is<string>(s => s == userId))).ReturnsAsync(true);

            var store = new InMemoryCachedSettingsDataStore(innerStore.Object, cache, new InMemoryCachingOptions());
            var result = await store.DoesExistAsync(userId);

            Assert.True(result);

            Assert.Equal(0, cache.Count);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenFindingAndItemPresentThenStoreNotHitTest()
        {
            var userId = Guid.NewGuid().ToString();
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.GetOrCreate(GetKey(userId), entry => new UserSettings { UserId = userId });
            var innerStore = new Mock<ISettingsDataStore>();

            var store = new InMemoryCachedSettingsDataStore(innerStore.Object, cache, new InMemoryCachingOptions());
            var result = await store.FindAsync(userId);

            Assert.NotNull(result);

            innerStore.Verify(x => x.FindAsync(It.IsAny<string>()), Times.Never);
            Assert.Equal(1, cache.Count);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenFindingAndItemNotPresentThenStoreHitTest()
        {
            var userId = Guid.NewGuid().ToString();
            var cache = new MemoryCache(new MemoryCacheOptions());
            var innerStore = new Mock<ISettingsDataStore>();
            innerStore.Setup(x => x.FindAsync(It.Is<string>(s => s == userId)))
                .ReturnsAsync(new UserSettings { UserId = userId });

            var store = new InMemoryCachedSettingsDataStore(innerStore.Object, cache, new InMemoryCachingOptions());
            var result = await store.FindAsync(userId);
            var resultNext = await store.FindAsync(userId);

            Assert.NotNull(result);
            Assert.NotNull(resultNext);
            Assert.Equal(resultNext, result);
            Assert.True(cache.TryGetValue(GetKey(userId), out var cachedResult));
            Assert.NotNull(cachedResult);

            Assert.Equal(1, cache.Count);
            innerStore.Verify(x=>x.FindAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        [Trait("Category", "Unit")]
        public async Task WhenUpdatingThenRemovedFromCacheTest()
        {
            var userId = Guid.NewGuid().ToString();
            var cache = new MemoryCache(new MemoryCacheOptions());
            cache.GetOrCreate(GetKey(userId), entry => new UserSettings { UserId = userId });

            var innerStore = new Mock<ISettingsDataStore>();
            UserSettings newSettings = null;
            innerStore.Setup(x => x.UpdateAsync(It.IsAny<UserSettings>()))
                .Callback<UserSettings>(settings => newSettings = settings)
                .ReturnsAsync(() => newSettings);

            var store = new InMemoryCachedSettingsDataStore(innerStore.Object, cache, new InMemoryCachingOptions());
            var result = await store.UpdateAsync(new UserSettings { UserId = userId });

            Assert.NotNull(result);

            innerStore.Verify(x => x.FindAsync(It.IsAny<string>()), Times.Never);
            Assert.Equal(0, cache.Count);
        }

        private static string GetKey(string userid)
        {
            return $"{typeof(UserSettings).Name.ToLower()}:{userid}";
        }
    }
}
