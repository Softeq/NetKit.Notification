// Developed by Softeq Development Corporation
// http://www.softeq.com

using AutoMapper;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Linq;
using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Domain.Models;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using Softeq.NetKit.Notifications.Store.CosmosDB.Client;
using Softeq.NetKit.Notifications.Store.CosmosDB.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.DataStores
{
    internal class NotificationRecordDataStore : BaseDataStore<Models.NotificationRecord>, INotificationHistoryDataStore
    {
        public NotificationRecordDataStore(ICosmosDbClientFactory factory, StorageConfiguration configuration, IMapper mapper)
            : base(factory, configuration, mapper)
        {
        }

        public async Task<NotificationRecord> SaveAsync(NotificationRecord record)
        {
            try
            {
                var collectionUri = GetCollectionUri();

                var storeEntity = new Models.NotificationRecord();
                storeEntity = Mapper.Map(record, storeEntity);

                storeEntity.Created = DateTimeOffset.UtcNow;
                storeEntity.Id = Guid.NewGuid();
                await Client.CreateDocumentAsync(collectionUri, storeEntity);

                return Mapper.Map<Models.NotificationRecord, NotificationRecord>(storeEntity);
            }
            catch (DocumentClientException e)
            {
                throw GetException(e);
            }
        }

        public async Task<NotificationRecord> FindAsync(Guid id)
        {
            var record = await FindInternalAsync(id);
            return record == null
                ? null
                : Mapper.Map<Models.NotificationRecord, NotificationRecord>(record);
        }

        public async Task<IEnumerable<NotificationRecord>> ListAsync(string userId, int pageSize, FilterOptions options)
        {
            var notificationsCollectionUri = GetCollectionUri();
            var baseQuery = Client.CreateDocumentQuery<Models.NotificationRecord>(notificationsCollectionUri, new FeedOptions
                {
                    MaxItemCount = pageSize,
                    PartitionKey = ResolvePartitionKey(userId)
                })
                .Where(record => record.OwnerUserId == userId);

            if (options.StartTime.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Created >= options.StartTime.Value);
            }
            if (options.EndTime.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.Created <= options.EndTime.Value);
            }
            var query = baseQuery.OrderByDescending(x => x.Created)
                .Take(pageSize)
                .AsDocumentQuery();

            var models = new List<Models.NotificationRecord>();

            while (query.HasMoreResults)
            {
                foreach (var notificationDocument in await query.ExecuteNextAsync<Models.NotificationRecord>())
                {
                    models.Add(notificationDocument);
                }
            }

            return models.Select(Mapper.Map<NotificationRecord>).ToList();
        }

        public Task DeleteAllAsync(string userId)
        {
            var storedProcedureUri = GetCollectionStoredProcedureUri(Configuration.BulkDeleteNotificationStoredProcedureId);
            return Client.ExecuteStoredProcedureAsync<string>(storedProcedureUri, userId);
        }
    }
}
