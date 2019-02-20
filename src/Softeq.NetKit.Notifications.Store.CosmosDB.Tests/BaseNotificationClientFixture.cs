// Developed by Softeq Development Corporation
// http://www.softeq.com

using Softeq.NetKit.Notifications.Domain.DataStores;
using Softeq.NetKit.Notifications.Store.CosmosDB.Client;
using Softeq.NetKit.Notifications.Store.CosmosDB.DataStores;
using Softeq.NetKit.Notifications.Store.CosmosDB.Tests.Utility;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Tests
{
    public class BaseNotificationClientFixture
    {
        public BaseNotificationClientFixture()
        {
            Store = CreateNotificationClient();
        }

        protected INotificationHistoryDataStore Store { get; }

        private INotificationHistoryDataStore CreateNotificationClient()
        {
            var options = ConfigurationHelper.GetDefaultDocumentDbNotification();
            var notificationClient = new NotificationRecordDataStore(new CosmosDbClientFactory(options), options, null);
            return notificationClient;
        }
    }
}
