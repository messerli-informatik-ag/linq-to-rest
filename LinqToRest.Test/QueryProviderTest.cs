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
            var actual = CreateQuery<EntityWithQueryableMember>()
                .ToString();

            var expected = EntityWithQueryableMemberResult;

            Assert.Equal(actual, expected.Query);
        }

        [Fact]
        public void ReturnsRestQueryWithSelect()
        {
            var actual = CreateQuery<EntityWithQueryableMember>()
                .Select(entity => new { entity.Name })
                .ToString();

            var expected = UniqueIdentifyerNameResult;

            Assert.Equal(actual, expected.Query);
        }

        [Fact]
        public void ReturnsRestQueryWithSelectedUniqueIdentifier()
        {
            var actual = CreateQuery<EntityWithQueryableMember>()
                .Select(entity => new { entity.UniqueIdentifier, entity.Name })
                .ToString();

            var expected = UniqueIdentifyerNameResult;

            Assert.Equal(actual, expected.Query);
        }

        [Fact]
        public void ReturnsRestObject()
        {
            var actual = new QueryResult<object>(
                CreateQuery<EntityWithQueryableMember>());

            var expected = EntityWithQueryableMemberResult;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsRestObjectWithSelect()
        {
            var actual = new QueryResult<object>(
                CreateQuery<EntityWithQueryableMember>()
                    .Select(entity => new { entity.Name }));

            var expected = UniqueIdentifyerNameResult;

            Assert.Equal(actual, expected);
        }

        [Fact]
        public void ReturnsRestObjectWithSelectedUniqueIdentifier()
        {
            var actual = new QueryResult<object>(
                CreateQuery<EntityWithQueryableMember>()
                    .Select(entity => new { entity.UniqueIdentifier, entity.Name }));

            var expected = UniqueIdentifyerNameResult;

            Assert.Equal(actual, expected);
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
                EntityWithQueryableMemberRequestUri,
                EntityWithQueryableMemberDeserialized);

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

        private static object EntityWithQueryableMemberDeserialized => new[]
        {
            new EntityWithQueryableMember("Test1", null),
            new EntityWithQueryableMember("Test2", null)
        };

        private static Uri EntityWithQueryableMemberRequestUri => new Uri(RootUri, "entitywithqueryablemembers");

        private static QueryResult<object> EntityWithQueryableMemberResult => new QueryResult<object>(
            EntityWithQueryableMemberRequestUri,
            new object[]
            {
                new EntityWithQueryableMember(
                    "Test1",
                    new Query<EntityWithSimpleMembers>(
                        MockQueryProviderFactory().Create(new Uri(RootUri, "entitywithqueryablemembers/Test1/")))),
                new EntityWithQueryableMember(
                    "Test2",
                    new Query<EntityWithSimpleMembers>(
                        MockQueryProviderFactory().Create(new Uri(RootUri, "entitywithqueryablemembers/Test2/"))))
            });

        private static Uri UniqueIdentifyerNameRequestUri => new Uri(RootUri, "entitywithqueryablemembers?fields=uniqueIdentifier,name");

        private static QueryResult<object> UniqueIdentifyerNameResult => new QueryResult<object>(
            UniqueIdentifyerNameRequestUri,
            new object[]
            {
                new
                {
                    UniqueIdentifier = "Test1",
                    Name = "Test1"
                },
                new
                {
                    UniqueIdentifier = "Test2",
                    Name = "Test2"
                }
            });

        #endregion
    }
}
