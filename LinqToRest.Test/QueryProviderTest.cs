using System;
using System.Linq;
using System.Net.Http;
using Messerli.LinqToRest.Test.Stub;
using Messerli.QueryProvider;
using NSubstitute;
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

            var expected = EntityWithQueryableMemberResult.Query;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsRestQueryWithSelect()
        {
            var actual = CreateQuery<EntityWithQueryableMember>()
                .Select(entity => new { entity.Name })
                .ToString();

            var expected = UniqueIdentifierNameResult.Query;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsRestQueryWithSelectedUniqueIdentifier()
        {
            var actual = CreateQuery<EntityWithQueryableMember>()
                .Select(entity => new { entity.UniqueIdentifier, entity.Name })
                .ToString();

            var expected = UniqueIdentifierNameResult.Query;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReturnsRestObject()
        {
            var actual = new QueryResult<EntityWithQueryableMember>(
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

        private static IQueryable<T> CreateQuery<T>()
        {
            return new Query<T>(CreateQueryProvider(RootUri));
        }

        private static IQueryable<T> CreateQuery<T>(string subPath)
        {
            return new Query<T>(CreateQueryProvider(new Uri(RootUri, subPath)));
        }

        private static QueryProvider CreateQueryProvider(Uri root)
        {
            return new QueryProvider(CreateResourceRetriever(), null, () => new QueryBinder(new EntityValidator()), root);
        }

        #endregion

        #region Mock

        private static HttpClient MockHttpClient()
        {
            return new HttpClientMock()
                .RegisterJsonResponse(UniqueIdentifierNameRequestUri.ToString(), UniqueIdentifierNameJson)
                .RegisterJsonResponse(EntityWithQueryableMemberRequestUri.ToString(), EntityWithQueryableMemberJson)
                .ToHttpClient();
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
        ""uniqueIdentifier"": ""Test2"",
        ""name"": ""Test2""
    }
]
";

        private static QueryResult<EntityWithQueryableMember> EntityWithQueryableMemberResult => new QueryResult<EntityWithQueryableMember>(
            EntityWithQueryableMemberRequestUri,
            new[]
            {
                new EntityWithQueryableMember("Test1", CreateQuery<EntityWithSimpleMembers>("entitywithqueryablemembers/Test1/")),
                new EntityWithQueryableMember("Test2", CreateQuery<EntityWithSimpleMembers>("entitywithqueryablemembers/Test2/")),
            });

        private static ResourceRetriever CreateResourceRetriever()
        {
            return new ResourceRetriever(
                MockHttpClient(),
                (type, uri) =>
                {
                    var queryProvider = new QueryProvider(
                        Substitute.For<IResourceRetriever>(),
                        null,
                        () => new QueryBinder(new EntityValidator()),
                        uri);

                    return Activator.CreateInstance(typeof(Query<>).MakeGenericType(type), queryProvider) as IQueryable<object>;
                });
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
}
