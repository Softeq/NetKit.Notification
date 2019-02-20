// Developed by Softeq Development Corporation
// http://www.softeq.com

using System;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Softeq.NetKit.Notifications.Domain.Exceptions;
using Softeq.NetKit.Notifications.Domain.Models.Errors;
using Softeq.NetKit.Notifications.Store.CosmosDB.Client;
using Softeq.NetKit.Notifications.Store.CosmosDB.Setup;

namespace Softeq.NetKit.Notifications.Store.CosmosDB.DataStores
{
    //TODO : build an abstraction around IDocumentClient ot make data store class unit-testable
    internal abstract class BaseDataStore<TModel>
    {
        protected IMapper Mapper;
        protected readonly StorageConfiguration Configuration;
        protected IDocumentClient Client;

        protected BaseDataStore(ICosmosDbClientFactory factory, StorageConfiguration configuration, IMapper mapper)
        {
            Mapper = mapper;
            Configuration = configuration;
            Client = factory.CreateClient();
            CollectionName = typeof(TModel).Name;
        }

        protected static Exception GetException(DocumentClientException e)
        {
            if (e.StatusCode == HttpStatusCode.Conflict)
            {
                return new ServiceException(e, new ErrorDto(ErrorCode.ConflictError, "Entity already exists."));
            }

            if (e.StatusCode == HttpStatusCode.NotFound)
            {
                return new ServiceException(e, new ErrorDto(ErrorCode.NotFound, "Entity not found."));
            }

            return new ServiceException(e);
        }

        protected async Task<TModel> FindInternalAsync(Guid id)
        {
            try
            {
                var documentUri = GetDocumentUri(id.ToString());
                var document = await Client.ReadDocumentAsync<TModel>(documentUri);

                return document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return default(TModel);
                }

                throw GetException(e);
            }
        }

        protected readonly string CollectionName;

        public virtual PartitionKey ResolvePartitionKey(string key) => null;

        protected Uri GetCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(Configuration.DatabaseId, CollectionName);
        }

        protected Uri GetDocumentUri(string documentId)
        {
            return UriFactory.CreateDocumentUri(Configuration.DatabaseId, CollectionName, documentId);
        }

        protected Uri GetCollectionStoredProcedureUri(string spName)
        {
            return UriFactory.CreateStoredProcedureUri(Configuration.DatabaseId, CollectionName, spName);
        }
    }
}
