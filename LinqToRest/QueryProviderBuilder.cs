using System;
using System.Net.Http;
using JetBrains.Annotations;
using QueryProviderBase = Messerli.QueryProvider.QueryProvider;

namespace Messerli.LinqToRest
{
    public class QueryProviderBuilder
    {
        private HttpClient _httpClient = new HttpClient();
        [CanBeNull] private Uri _uri;

        public QueryProviderBuilder Root(Uri uri)
        {
            _uri = uri;
            return this;
        }
        
        public QueryProviderBuilder HttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            return this;
        }
        
        public QueryProviderBase Build()
        {
            var resourceRetriever = CreateResourceRetriever();
            var queryBinderFactory = CreateQueryBinderFactory();
            
            if (_uri is null)
            {
                throw new QueryProviderBuilderException($"Root uri was not configured. Call .{nameof(Root)}(...) before .{nameof(Build)}().");
            }
            return new QueryProvider(resourceRetriever, queryBinderFactory, _uri);
        }

        private static QueryableFactory CreateQueryableFactory()
        {
            return (type, uri) =>
            {
                throw new NotImplementedException();
            };
        }

        private ResourceRetriever CreateResourceRetriever()
        {
            var queryableFactory = CreateQueryableFactory();
            return new ResourceRetriever(_httpClient, queryableFactory);
        }

        private static QueryBinderFactory CreateQueryBinderFactory()
        {
            var entityValidator = CreateEntityValidator();
            return () => new QueryBinder(entityValidator);
        }

        private static EntityValidator CreateEntityValidator()
        {
            return new EntityValidator();
        }
    }
}