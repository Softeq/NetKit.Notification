// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Softeq.NetKit.Notifications.Store.CosmosDB.Setup;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Tests.Utility
{
    public static class ConfigurationHelper
    {
        private static readonly Uri DocumentDbServiceEndpoint = new Uri("COSMOSDB_CONNSTR");
        private static readonly string DocumentDbAuthKey = "COSMOSDB_KEY";

        public static IDocumentClient CreateDocumentDbClient()
        {
            var client = new DocumentClient(DocumentDbServiceEndpoint, DocumentDbAuthKey);
            return client;
        }

        public static StorageConfiguration GetDefaultDocumentDbNotification()
        {
            return new StorageConfiguration
            {
                NotificationTtlDays = 60,
                Endpoint = DocumentDbServiceEndpoint,
                Key = DocumentDbAuthKey,
                DatabaseId = "softeq-netkit-notification-test",
                BulkDeleteNotificationStoredProcedureId = "bulkDeleteNotifications",
                DefaultRUs = 400
            };
        }
    }
}
