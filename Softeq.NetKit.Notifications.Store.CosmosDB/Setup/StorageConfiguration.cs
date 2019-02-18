// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Setup
{
    public class StorageConfiguration
    {
        public int NotificationTtlDays { get; set; }

        public Uri Endpoint { get; set; }

        public string Key { get; set; }

        public string DatabaseId { get; set; }

        public string BulkDeleteNotificationStoredProcedureId { get; set; }

        public int DefaultRUs { get; set; }
    }
}