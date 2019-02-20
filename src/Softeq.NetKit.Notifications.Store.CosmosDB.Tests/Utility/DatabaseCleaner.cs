// Developed by Softeq Development Corporation
// http://www.softeq.com

using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Tests.Utility
{
    public static class DatabaseCleaner
    {
        public static async Task CleanupTestDatabaseAsync()
        {
            var options = ConfigurationHelper.GetDefaultDocumentDbNotification();
            var notificationCollectionUri = UriFactory.CreateDocumentCollectionUri(options.DatabaseId, typeof(Models.NotificationRecord).Name);
            var docDbClient = ConfigurationHelper.CreateDocumentDbClient();

            // clean db
            var docQuery = docDbClient.CreateDocumentQuery(notificationCollectionUri).AsDocumentQuery();
            while (docQuery.HasMoreResults)
            {
                var notificationsToDelete = await docQuery.ExecuteNextAsync<Document>();
                foreach (var notification in notificationsToDelete)
                {
                    await docDbClient.DeleteDocumentAsync(notification.SelfLink);
                }
            }
        }
    }
}
