using Messerli.LinqToRest.Test.Stub;
using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using RestQueryProvider = Messerli.LinqToRest.QueryProvider;

namespace Messerli.LinqToRest.Test
{
    public class QueryProviderTest
    {
        [Fact]
        public void ReturnsRestQuery()
        {
            var query = CreateQuery<EntityWithQueryableMember>();
            var restQuery = query.ToString();
            var expectedRestQuery = EntityWithQueryableMemberRequest.AbsoluteUri;

            Assert.Equal(expectedRestQuery, restQuery);
        }

        [Fact]
        public void ReturnsRestQueryWithSelect()
        {
            var query = CreateQuery<EntityWithQueryableMember>();
            var restQuery = query.Select(entity => new { entity.Name }).ToString();
            var expectedRestQuery = new Uri(EntityWithQueryableMemberRequest, "?fields=uniqueIdentifier,name").ToString();

            Assert.Equal(expectedRestQuery, restQuery);
        }

        [Fact]
        public void ReturnsRestQueryWithSelectedUniqueIdentifier()
        {
            var query = CreateQuery<EntityWithQueryableMember>();
            var restQuery = query.Select(entity => new { entity.UniqueIdentifier, entity.Name }).ToString();
            var expectedRestQuery = new Uri(EntityWithQueryableMemberRequest, "?fields=uniqueIdentifier,name").ToString();

            Assert.Equal(expectedRestQuery, restQuery);
        }

        [Fact]
        public void ReturnsRestObject()
        {
            var actual = new QueryResult<object>(
                CreateQuery<EntityWithQueryableMember>());

            var expected = new QueryResult<object>(
                EntityWithQueryableMemberRequest,
                EntityWithQueryableMemberResult);

            Assert.Equal(expected, actual);
        }

        #region Helper

        private static Query<T> CreateQuery<T>()
        {
            var serviceUri = MockServiceUri();
            var resourceRetriever = MockResourceRetriever();
            var objectResolver = MockObjectResolver();
            var queryBinderFactory = MockQueryBinderFactory();

            var queryProvider = new RestQueryProvider(resourceRetriever, objectResolver, queryBinderFactory, serviceUri);

            return new Query<T>(queryProvider);
        }

        #endregion

        #region Mock

        private static Uri MockServiceUri()
        {
            return RootUri;
        }

        private static IResourceRetriever MockResourceRetriever()
        {
            var retriever = Substitute.For<IResourceRetriever>();

            retriever = AddUriMock<IEnumerable<EntityWithQueryableMember>>(retriever,
                EntityWithQueryableMemberRequest,
                ResourceRetrieverEntityWithQueryableMemberResult);

            return retriever;
        }

        private static IResourceRetriever AddUriMock<T>(IResourceRetriever resourceRetriever, Uri uri, object value)
        {
            resourceRetriever.RetrieveResource<T>(uri).Returns(Task.FromResult((T)value));

            return resourceRetriever;
        }

        private static IObjectResolver MockObjectResolver()
        {
            var queryableFactory = new QueryableFactory(MockQueryProviderFactory());

            return new QueryableObjectResolver(queryableFactory);
        }

        private static QueryProviderFactory MockQueryProviderFactory()
        {
            return new QueryProviderFactory(
                MockResourceRetriever(),
                // Todo: resolve circular dependency!
                new DefaultObjectResolver(),
                MockQueryBinderFactory(),
                MockServiceUri());
        }

        private static QueryBinderFactory MockQueryBinderFactory()
        {
            var queryBinderFactory = Substitute.For<QueryBinderFactory>();
            queryBinderFactory().Returns(new QueryBinder(new EntityValidator()));

            return queryBinderFactory;
        }

        #endregion

        #region Data

        private static Uri RootUri => new Uri("http://www.exapmle.com/api/v1/", UriKind.Absolute);

        private static Uri EntityWithQueryableMemberRequest => new Uri(RootUri, "entitywithqueryablemembers");

        private static Uri EntityWithQueryableMemberTest1Root => new Uri(RootUri, "entitywithqueryablemembers/Test1/");

        private static Uri EntityWithQueryableMemberTest2Root => new Uri(RootUri, "entitywithqueryablemembers/Test2/");

        private static object ResourceRetrieverEntityWithQueryableMemberResult => new[]
        {
            new EntityWithQueryableMember("Test1", null),
            new EntityWithQueryableMember("Test2", null)
        };

        private static object[] EntityWithQueryableMemberResult => new object[]
        {
            new EntityWithQueryableMember("Test1", new Query<EntityWithSimpleMembers>(MockQueryProviderFactory().Create(EntityWithQueryableMemberTest1Root))),
            new EntityWithQueryableMember("Test2", new Query<EntityWithSimpleMembers>(MockQueryProviderFactory().Create(EntityWithQueryableMemberTest2Root)))
        };

        #endregion
    }
}
