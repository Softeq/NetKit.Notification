// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Extensions.Caching.Memory;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Models.NotificationSettings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Softeq.NetKit.Notifications.Service.Services.Caching
{
    internal class InMemoryCachedSettingsDataStore : ISettingsDataStore
    {
        private readonly ISettingsDataStore _originalDataStore;
        private readonly IMemoryCache _cache;
        private readonly InMemoryCachingOptions _options;
        private static readonly SemaphoreSlim sync = new SemaphoreSlim(1);

        public InMemoryCachedSettingsDataStore(ISettingsDataStore originalDataStore, IMemoryCache cache, InMemoryCachingOptions options)
        {
            _originalDataStore = originalDataStore;
            _cache = cache;
            _options = options;
        }

        public Task<bool> DoesExistAsync(string userId)
        {
            return _cache.TryGetValue(GetKey(userId), out _) 
                ? Task.FromResult(true) 
                : _originalDataStore.DoesExistAsync(userId);
        }

        public async Task<UserSettings> FindAsync(string userId)
        {
            var key = GetKey(userId);

            if (_cache.TryGetValue(key, out UserSettings settings))
            {
                return settings;
            }

            await sync.WaitAsync();

            try
            {
                if (_cache.TryGetValue(key, out settings))
                {
                    return settings;
                }

                settings = await _originalDataStore.FindAsync(userId);
                if (settings == null)
                {
                    return null;
                }

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.UtcNow.AddHours(_options.UserSettingsExpiresInHours),
                    Priority = CacheItemPriority.Normal
                };

                _cache.Set(key, settings, options);
                return settings;
            }
            finally
            {
                sync.Release();
            }
        }

        public Task<UserSettings> SaveAsync(UserSettings settings)
        {
            return _originalDataStore.SaveAsync(settings);
        }

        public async Task<UserSettings> UpdateAsync(UserSettings settings)
        {
            await sync.WaitAsync();

            try
            {
                var newSettings = await _originalDataStore.UpdateAsync(settings);
                _cache.Remove(GetKey(settings.UserId));
                return newSettings;
            }
            finally
            {
                sync.Release();
            }
        }

        private static string GetKey(string userid)
        {
            return $"{cachePrefix}:{userid}";
        }

        private static readonly string cachePrefix = typeof(UserSettings).Name.ToLower();
    }
}
