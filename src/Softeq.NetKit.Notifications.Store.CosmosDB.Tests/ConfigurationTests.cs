// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Linq;
using System.Threading.Tasks;
using Softeq.NetKit.Notifications.Store.CosmosDB.Client;
using Softeq.NetKit.Notifications.Store.CosmosDB.Setup;
using Softeq.NetKit.Notifications.Store.CosmosDB.Tests.Utility;
using Xunit;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Tests
{
    public class ConfigurationTests
    {
        public ConfigurationTests()
        {
            var options = ConfigurationHelper.GetDefaultDocumentDbNotification();
            var docDbClient = ConfigurationHelper.CreateDocumentDbClient();
            var database = docDbClient.CreateDatabaseQuery()
                .Where(db => db.Id == options.DatabaseId)
                .AsEnumerable()
                .FirstOrDefault();

            if (database != null)
            {
                docDbClient.DeleteDatabaseAsync(database.SelfLink).GetAwaiter();
            }
        }

        [Fact]
        public async Task ShouldCreateDatabaseAndCollectionIfNotExists()
        {
            var options = ConfigurationHelper.GetDefaultDocumentDbNotification();
            var docDbClient = ConfigurationHelper.CreateDocumentDbClient();
            var bootstarapper = new StorageBootstrapper(new CosmosDbClientFactory(options), options);

            await bootstarapper.RunAsync();

            var databaseId = options.DatabaseId;
            var createdDatabase = docDbClient.CreateDatabaseQuery()
                .Where(db => db.Id == databaseId)
                .AsEnumerable()
                .FirstOrDefault();

            Assert.NotNull(createdDatabase);
            Assert.Equal(databaseId, createdDatabase.Id);

            var collectionId = typeof(Models.NotificationRecord).Name;
            var createdCollection = docDbClient.CreateDocumentCollectionQuery(createdDatabase.SelfLink)
                .Where(c => c.Id == collectionId)
                .AsEnumerable()
                .FirstOrDefault();

            Assert.NotNull(createdCollection);
            Assert.Equal(collectionId, createdCollection.Id);

            var includedIndexingPaths = createdCollection.IndexingPolicy.IncludedPaths;

            Assert.True(includedIndexingPaths.Count == 3);
        }
    }
}
