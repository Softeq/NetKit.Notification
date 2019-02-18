// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Softeq.NetKit.Notifications.Domain.Infrastructure;
using Softeq.NetKit.Notifications.Domain.Models.NotificationRecord;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Store.CosmosDB.Client;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Setup
{
    internal class StorageBootstrapper : IBootstrapper
    {
        private readonly IDocumentClient _client;
        private readonly StorageConfiguration _configuration;

        public StorageBootstrapper(ICosmosDbClientFactory factory, StorageConfiguration configuration)
        {
            _client = factory.CreateClient();
            _configuration = configuration;
        }

        public async Task RunAsync()
        {
            await SetupDatabaseAsync();
            await SetupCollectionsAsync();
            await SetupStoredProcedureAsync();
        }

        private async Task SetupDatabaseAsync()
        {
            var databaseId = _configuration.DatabaseId;
            var database = _client.CreateDatabaseQuery()
                                  .Where(db => db.Id == databaseId)
                                  .AsEnumerable()
                                  .FirstOrDefault();

            if (database == null)
            {
                database = new Database { Id = databaseId };
                await _client.CreateDatabaseAsync(database);
            }
        }

        private async Task SetupCollectionsAsync()
        {
            await SetupNotificationHistoryCollection();
            await SetupUserSettingsCollection();
        }

        private async Task SetupUserSettingsCollection()
        {
            var collectionName = typeof(Models.UserSettings).Name;
            var databaseUri = UriFactory.CreateDatabaseUri(_configuration.DatabaseId);

            var collection = _client.CreateDocumentCollectionQuery(databaseUri)
                                                 .Where(c => c.Id == collectionName)
                                                 .AsEnumerable()
                                                 .FirstOrDefault();
            
            if (collection == null)
            {
                collection = new DocumentCollection
                {
                    Id = collectionName,
                    IndexingPolicy = { IndexingMode = IndexingMode.Consistent }
                };

                collection.IndexingPolicy.IncludedPaths.Add(
                    new IncludedPath
                    {
                        Path = "/",
                        Indexes = new Collection<Index>
                        {
                            Index.Hash(DataType.String, -1)
                        }
                    });

                var codeProperty = nameof(Models.UserSettings.UserId);
                collection.IndexingPolicy.IncludedPaths.Add(
                    new IncludedPath
                    {
                        Path = $"/\"{codeProperty}\"/?",
                        Indexes = new Collection<Index>
                        {
                            Index.Hash(DataType.String, -1)
                        }
                    });

                var requestOptions = new RequestOptions
                {
                    OfferThroughput = _configuration.DefaultRUs
                };
                await _client.CreateDocumentCollectionAsync(databaseUri, collection, requestOptions);
            }
        }

        private async Task SetupNotificationHistoryCollection()
        {
            var collectionName = typeof(Models.NotificationRecord).Name;
            var databaseUri = UriFactory.CreateDatabaseUri(_configuration.DatabaseId);

            var collection = _client.CreateDocumentCollectionQuery(databaseUri)
                                                 .Where(c => c.Id == collectionName)
                                                 .AsEnumerable()
                                                 .FirstOrDefault();

            if (collection == null)
            {
                var ttlSeconds = (int)TimeSpan.FromDays(_configuration.NotificationTtlDays).TotalSeconds;
                collection = new DocumentCollection
                {
                    Id = collectionName,
                    DefaultTimeToLive = ttlSeconds,
                    IndexingPolicy = { IndexingMode = IndexingMode.Consistent }
                };

                collection.IndexingPolicy.IncludedPaths.Add(
                    new IncludedPath
                    {
                        Path = "/",
                        Indexes = new Collection<Index>
                        {
                            Index.Hash(DataType.String, -1)
                        }
                    });

                var createdPropertyName = nameof(Models.NotificationRecord.Created);
                collection.IndexingPolicy.IncludedPaths.Add(
                    new IncludedPath
                    {
                        Path = $"/\"{createdPropertyName}\"/?",
                        Indexes = new Collection<Index>
                        {
                            Index.Range(DataType.String, -1)
                        }
                    });

                var ownerPropertyName = nameof(Models.NotificationRecord.OwnerUserId);
                collection.IndexingPolicy.IncludedPaths.Add(
                    new IncludedPath
                    {
                        Path = $"/\"{ownerPropertyName}\"/?",
                        Indexes = new Collection<Index>
                        {
                            Index.Hash(DataType.String, -1)
                        }
                    });

                var requestOptions = new RequestOptions
                {
                    OfferThroughput = _configuration.DefaultRUs
                };
                await _client.CreateDocumentCollectionAsync(databaseUri, collection, requestOptions);
            }
        }

        private async Task SetupStoredProcedureAsync()
        {
            var collectionName = typeof(NotificationRecord).Name;
            var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(_configuration.DatabaseId, collectionName);
            var storedProcedure = _client.CreateStoredProcedureQuery(documentCollectionUri)
                .Where(db => db.Id == _configuration.BulkDeleteNotificationStoredProcedureId)
                .AsEnumerable()
                .FirstOrDefault();

            if (storedProcedure == null)
            {
                storedProcedure = new StoredProcedure()
                {
                    Id = _configuration.BulkDeleteNotificationStoredProcedureId,
                    Body = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts/bulkDeleteNotifications.js"))
                };
                await _client.CreateStoredProcedureAsync(documentCollectionUri, storedProcedure);
            }
        }
    }
}
