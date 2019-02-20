// Developed by Softeq Development Corporation
// http://www.softeq.com

using Microsoft.Azure.Documents;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.Client
{
    internal interface ICosmosDbClientFactory
    {
        IDocumentClient CreateClient();
    }
}
