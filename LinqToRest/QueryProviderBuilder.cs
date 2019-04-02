using System;
using System.Linq;
using System.Net.Http;
using JetBrains.Annotations;
using Messerli.QueryProvider;
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
            
            var queryProvider = new QueryProvider(resourceRetriever, queryBinderFactory, _uri);
            var queryableFactory = CreateQueryableFactory(queryProvider);
            resourceRetriever.QueryableFactory = queryableFactory;

            return queryProvider;
        }

        private static QueryableFactory CreateQueryableFactory(QueryProvider subQueryProvider)
        {
            return (type, uri) => Activator.CreateInstance(typeof(Query<>).MakeGenericType(type), subQueryProvider) as IQueryable<object>;
        }

        private ResourceRetriever CreateResourceRetriever()
        {
            return new ResourceRetriever(_httpClient);
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