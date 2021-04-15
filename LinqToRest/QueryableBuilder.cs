using System;
using System.Linq;
using System.Net.Http;
using JetBrains.Annotations;

namespace Messerli.LinqToRest
{
    public class QueryableBuilder: IQueryableBuilder
    {
        private HttpClient _httpClient = new HttpClient();
        private INamingPolicy _resourceNamingPolicy = NamingPolicy.LowerCasePlural;
        [CanBeNull] private Uri _uri;

        public IQueryableBuilder Root(Uri uri)
        {
            _uri = uri;
            return this;
        }

        public IQueryableBuilder HttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            return this;
        }

        public IQueryableBuilder ResourceNamingPolicy(INamingPolicy namingPolicy)
        {
            _resourceNamingPolicy = namingPolicy;
            return this;
        }

        public IQueryable<T> Build<T>()
        {
            ValidateConfiguration();

            var resourceRetriever = CreateResourceRetriever();
            var queryBinderFactory = CreateQueryBinderFactory();

            var queryProvider = new QueryProvider(resourceRetriever, queryBinderFactory, _uri, _resourceNamingPolicy);
            var queryableFactory = CreateQueryableFactory(resourceRetriever, queryBinderFactory);

            // Resolve circular dependency
            resourceRetriever.QueryableFactory = queryableFactory;

            return new Query<T>(queryProvider);
        }

        public IResourceRetriever BuildResourceRetriever<T>() => CreateResourceRetriever();

        private void ValidateConfiguration()
        {
            if (_uri is null)
            {
                throw new QueryableBuilderException(
                    $"Root uri was not configured. Call .{nameof(Root)}(...) before .{nameof(Build)}().");
            }

            if (_httpClient is null)
            {
                throw new QueryableBuilderException(
                    $"HTTP client was configured as null. Call .{nameof(HttpClient)}(...) with a non-null value or leave it at its default value");
            }
        }

        private QueryableFactory CreateQueryableFactory(IResourceRetriever resourceRetriever, QueryBinderFactory queryBinderFactory)
        {
            return (type, uri) =>
            {
                var subQueryProvider = new QueryProvider(resourceRetriever, queryBinderFactory, uri, _resourceNamingPolicy);
                return Activator.CreateInstance(typeof(Query<>).MakeGenericType(type), subQueryProvider) as
                        IQueryable<object>;
            };
        }

        private ResourceRetriever CreateResourceRetriever() => new ResourceRetriever(_httpClient);

        private QueryBinderFactory CreateQueryBinderFactory()
        {
            var entityValidator = CreateEntityValidator();
            return () => new QueryBinder(entityValidator);
        }

        private EntityValidator CreateEntityValidator() => new EntityValidator();
    }
}
