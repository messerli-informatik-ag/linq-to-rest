using System;
using System.Collections.Generic;
using Messerli.LinqToRest.Test.Stub;
using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using NSubstitute;
using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class QueryProviderTest
    {
        [Fact]
        public void ReturnsRestQuery()
        {
            var serviceUri = MockServiceUri();

            var resourceRetriever = MockResourceRetriever();
            var queryBinderFactory = MockQueryBinderFactory();


            var queryProvider = new QueryProvider(resourceRetriever, queryBinderFactory, serviceUri);
            var query = new Query<EntityWithQueryableMember>(queryProvider);

            var restQuery = query.ToString();

            var expectedRestQuery = $"{serviceUri.AbsoluteUri}entitywithqueryablemembers";
            Assert.Equal(expectedRestQuery, restQuery);
        }

        private static Uri MockServiceUri()
        {
            const string root = "http://www.exapmle.com/api/v1/";
            return new UriBuilder(root).Uri;
        }

        private static IResourceRetriever MockResourceRetriever()
        {
            var resourceRetriever = Substitute.For<IResourceRetriever>();

            var uri = new Uri(MockServiceUri(), "test");
            var result = new[]
            {
                new EntityWithQueryableMember("Test1", null),
                new EntityWithQueryableMember("Test2", null)
            };
            resourceRetriever.RetrieveResource<IEnumerable<EntityWithQueryableMember>>(uri).Returns(result);

            return resourceRetriever;
        }

        private static IObjectResolver MockObjectResolver()
        {
            var queryableFactory = new QueryableFactory(MockQueryProvider());

            return new QueryableObjectResolver(queryableFactory);
        }

        private static QueryProvider MockQueryProvider()
        {
            return new QueryProvider(
                MockResourceRetriever(),
                MockQueryBinderFactory(),
                MockServiceUri());
        }

        private static QueryBinderFactory MockQueryBinderFactory()
        {
            var queryBinderFactory = Substitute.For<QueryBinderFactory>();
            queryBinderFactory().Returns(new QueryBinder(new EntityValidator()));

            return queryBinderFactory;
        }
    }
}
