// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Softeq.NetKit.Notifications.Store.CosmosDB.Setup;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Client
{
    internal class CosmosDbClientFactory : ICosmosDbClientFactory
    {
        private readonly StorageConfiguration _config;

        public CosmosDbClientFactory(StorageConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IDocumentClient CreateClient()
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return new DocumentClient(_config.Endpoint, _config.Key, settings);
        }
    }
}
