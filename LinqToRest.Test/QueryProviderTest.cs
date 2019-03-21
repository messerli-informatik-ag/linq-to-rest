using System;
using System.Collections.Generic;
using System.Linq;
using Messerli.LinqToRest.Test.Stub;
using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using Messerli.Utility.Extension;
using NSubstitute;
using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class QueryProviderTest
    {
        [Fact]
        public void ReturnsRestQuery()
        {
            var query = CreateQuery<EntityWithQueryableMember>();
            var restQuery = query.ToString();
            var expectedRestQuery = $"{MockServiceUri().AbsoluteUri}entitywithqueryablemembers";

            Assert.Equal(expectedRestQuery, restQuery);
        }


        [Fact]
        public void ReturnsRestObject()
        {
            var query = CreateQuery<EntityWithQueryableMember>();
            var queryResult = query.ToArray();

            var expectedQueryObject = new[]
            {
                new EntityWithQueryableMember("Foo", null)
            };

            // Assert.Equals() calls Query<T>.GetEnumerable().Equals() and not Query<T>.Equals()
            // which executes queries :(
            expectedQueryObject
                .Zip(queryResult, (expectedEntity, actualEntity) => new { expectedEntity, actualEntity })
                .ForEach(obj =>
                {
                    obj.actualEntity.GetType().GetPropertyValues(obj.actualEntity)
                        .Zip(obj.actualEntity.GetType().GetPropertyValues(obj.actualEntity),
                            (expectedProperty, actualProperty) => new { expectedProperty, actualProperty })
                        .ForEach(zip => AssertEquals(zip.expectedProperty, zip.actualProperty));
                });
        }

        private static Query<T> CreateQuery<T>()
        {
            var serviceUri = MockServiceUri();
            var resourceRetriever = MockResourceRetriever();
            var queryBinderFactory = MockQueryBinderFactory();
            var queryProvider = new QueryProvider(resourceRetriever, queryBinderFactory, serviceUri);

            return new Query<T>(queryProvider);
        }

        private static void AssertEquals(object expected, object actual)
        {
            var isEqual = expected.Equals(actual);
            Assert.True(isEqual);
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
