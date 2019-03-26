using System;
using System.Linq;
using System.Net.Http;
using Messerli.LinqToRest.Test.Stub;
using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using NSubstitute;
using RichardSzalay.MockHttp;
using Xunit;

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

            Assert.Equal(expected.Query, actual);
        }

        [Fact]
        public void ReturnsRestQueryWithSelect()
        {
            var actual = CreateQuery<EntityWithQueryableMember>()
                .Select(entity => new { entity.Name })
                .ToString();

            var expected = UniqueIdentifierNameResult;

            Assert.Equal(expected.Query, actual);
        }

        [Fact]
        public void ReturnsRestQueryWithSelectedUniqueIdentifier()
        {
            var actual = CreateQuery<EntityWithQueryableMember>()
                .Select(entity => new { entity.UniqueIdentifier, entity.Name })
                .ToString();

            var expected = UniqueIdentifierNameResult;

            Assert.Equal(expected.Query, actual);
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

            var expected = NameResult;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsRestObjectWithSelectedUniqueIdentifier()
        {
            var actual = new QueryResult<object>(
                CreateQuery<EntityWithQueryableMember>()
                    .Select(entity => new { entity.UniqueIdentifier, entity.Name }));

            var expected = UniqueIdentifierNameResult;

            Assert.Equal(expected, actual);
        }

        #region Helper

        private static Query<T> CreateQuery<T>()
        {
            var queryProvider = CreateQueryProvider();
            return new Query<T>(queryProvider);
        }

        #endregion

        #region Mock

        private static Uri MockServiceUri()
        {
            return RootUri;
        }

        private static HttpClient MockHttpClient()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp
                .RegisterJsonResponse(UniqueIdentifierNameRequestUri.ToString(), UniqueIdentifierNameJson)
                .RegisterJsonResponse(EntityWithQueryableMemberRequestUri.ToString(), EntityWithQueryableMemberJson);

            return mockHttp.ToHttpClient();
        }

        private static IResourceRetriever MockResourceRetriever()
        {
            return new ResourceRetriever(MockHttpClient(), Substitute.For<IQueryableFactory>());
        }


        private static IObjectResolver MockObjectResolver()
        {
            var queryableFactory = new QueryableFactory(CreateQueryProvider());

            return new QueryableObjectResolver(queryableFactory);
        }


        private static QueryBinderFactory MockQueryBinderFactory()
        {
            var queryBinderFactory = Substitute.For<QueryBinderFactory>();
            queryBinderFactory().Returns(new QueryBinder(new EntityValidator()));

            return queryBinderFactory;
        }

        #endregion

        #region Data

        private static Uri RootUri => new Uri("http://www.example.com/api/v1/", UriKind.Absolute);

        private static Uri EntityWithQueryableMemberRequestUri => new Uri(RootUri, "entitywithqueryablemembers");

        private static string EntityWithQueryableMemberJson => @"
[
    {
        ""uniqueIdentifier"": ""Test1"",
        ""name"": ""Test1""
    },
    {
        ""uniqueIdentifier"": ""Test1"",
        ""name"": ""Test2""
    }
]
";

        private static QueryResult<object> EntityWithQueryableMemberResult => new QueryResult<object>(
            EntityWithQueryableMemberRequestUri,
            new object[]
            {
                new EntityWithQueryableMember(
                    "Test1",
                    new Query<EntityWithSimpleMembers>(CreateQueryProvider())),
                        //MockQueryProviderFactory().Create(new Uri(RootUri, "entitywithqueryablemembers/Test1/")))),
                new EntityWithQueryableMember(
                    "Test2",
                    new Query<EntityWithSimpleMembers>(CreateQueryProvider()))
                        //MockQueryProviderFactory().Create(new Uri(RootUri, "entitywithqueryablemembers/Test2/"))))
            });

        private static QueryProvider CreateQueryProvider()
        {
            return new QueryProvider(CreateResourceRetriever(), null, () => new QueryBinder(new EntityValidator()), RootUri);
        }

        private static ResourceRetriever CreateResourceRetriever()
        {
            return new ResourceRetriever(
                MockHttpClient(),
                new QueryableFactory(
                    new QueryProvider(
                        Substitute.For<IResourceRetriever>(),
                        null,
                        () => new QueryBinder(new EntityValidator()),
                        RootUri)));
        }


        private static Uri UniqueIdentifierNameRequestUri =>
            new Uri(RootUri, "entitywithqueryablemembers?fields=uniqueIdentifier,name");

        private static string UniqueIdentifierNameJson => @"
[
    {
        ""uniqueIdentifier"": ""Test1"",
        ""name"": ""Test1""
    },
    {
        ""uniqueIdentifier"": ""Test2"",
        ""name"": ""Test2""
    }
]
";

        private static QueryResult<object> UniqueIdentifierNameResult => new QueryResult<object>(
            UniqueIdentifierNameRequestUri,
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
        private static QueryResult<object> NameResult => new QueryResult<object>(
            UniqueIdentifierNameRequestUri,
            new object[]
            {
                new
                {
                    Name = "Test1"
                },
                new
                {
                    Name = "Test2"
                }
            });
        #endregion
    }

    internal static class Extension
    {
        public static MockHttpMessageHandler RegisterJsonResponse(
            this MockHttpMessageHandler httpMessageHandler,
            string route,
            string jsonResponse)
        {
            httpMessageHandler.When(route).Respond("application/json", jsonResponse);
            return httpMessageHandler;
        }
    }

}
