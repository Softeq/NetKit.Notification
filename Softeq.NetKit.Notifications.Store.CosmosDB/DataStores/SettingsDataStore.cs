// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Store.CosmosDB.Client;
using Softeq.NetKit.Notifications.Store.CosmosDB.Models;
using Softeq.NetKit.Notifications.Store.CosmosDB.Setup;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DomainUserSettings = Softeq.NetKit.Notifications.Domain.Models.NotificationSettings.UserSettings;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.DataStores
{
    internal class SettingsDataStore : BaseDataStore<UserSettings>, ISettingsDataStore
    {
        public SettingsDataStore(ICosmosDbClientFactory factory, StorageConfiguration configuration, IMapper mapper)
            : base(factory, configuration, mapper)
        {
        }

        public async Task<bool> DoesExistAsync(string userId)
        {
            var item = await FindAsync(userId, profile => profile.Id);
            return item != Guid.Empty;
        }

        public async Task<DomainUserSettings> FindAsync(string userId)
        {
            var item = await FindAsync(userId, profile => profile);
            return item == null
                ? null
                : Mapper.Map<UserSettings, DomainUserSettings>(item);
        }

        public async Task<DomainUserSettings> SaveAsync(DomainUserSettings settings)
        {
            try
            {
                var collectionUri = GetCollectionUri();

                var storeEntity = new UserSettings();
                storeEntity = Mapper.Map(settings, storeEntity);

                storeEntity.Created = DateTimeOffset.UtcNow;
                storeEntity.Updated = storeEntity.Created;
                storeEntity.Id = Guid.NewGuid();
                await Client.CreateDocumentAsync(collectionUri, storeEntity);

                return Mapper.Map<UserSettings, DomainUserSettings>(storeEntity);
            }
            catch (DocumentClientException e)
            {
                throw GetException(e);
            }
        }

        public async Task<DomainUserSettings> UpdateAsync(DomainUserSettings settings)
        {
            try
            {
                var documentUri = GetDocumentUri(settings.Id.ToString());

                var storeEntity = Mapper.Map<DomainUserSettings, UserSettings>(settings);
                storeEntity.Updated = DateTimeOffset.UtcNow;

                await Client.ReplaceDocumentAsync(documentUri, storeEntity);

                return Mapper.Map<UserSettings, DomainUserSettings>(storeEntity);
            }
            catch (DocumentClientException e)
            {
                throw GetException(e);
            }
        }

        private async Task<TResult> FindAsync<TResult>(string userId, Expression<Func<UserSettings, TResult>> selector)
        {
            var notificationsCollectionUri = GetCollectionUri();
            var query = Client.CreateDocumentQuery<UserSettings>(notificationsCollectionUri, new FeedOptions
                {
                    MaxItemCount = 1,
                    PartitionKey = ResolvePartitionKey(userId)
                })
                .Where(p => p.UserId == userId)
                .Take(1)
                .Select(selector)
                .AsDocumentQuery();

            var res = await query.ExecuteNextAsync<TResult>();
            return res.FirstOrDefault();
        }
    }
}
