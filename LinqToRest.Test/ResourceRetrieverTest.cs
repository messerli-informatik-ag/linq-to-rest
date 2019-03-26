using System;
using System.Collections.Generic;
using System.Net.Http;
using Messerli.LinqToRest.Test.Stub;
using Messerli.QueryProvider;
using NSubstitute;
using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class ResourceRetrieverTest
    {
        [Fact]
        public async void ReturnsRestObject()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(EntityWithQueryableMemberResult.Query, UriKind.Absolute);
            var actual = await resourceRetriever.RetrieveResource<IEnumerable<EntityWithQueryableMember>>(uri);

            Assert.Equal(EntityWithQueryableMemberResult.Object, actual);
        }

        [Fact]
        public async void ReturnsRestObjectWithSelect()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(UniqueIdentifierNameResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { Name = string.Empty }.GetType());
            var actual = await resourceRetriever.RetrieveResource(type, uri);

            Assert.Equal(UniqueIdentifierNameResult.Object, actual);
        }

        [Fact]
        public async void ReturnsRestObjectWithSelectedUniqueIdentifier()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(UniqueIdentifierNameResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { UniqueIdentifyer = string.Empty, Name = string.Empty }.GetType());
            var actual = await resourceRetriever.RetrieveResource(type, uri);

            Assert.Equal(UniqueIdentifierNameResult.Object, actual);
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

        #region Mock

        private static HttpClient MockHttpClient()
        {
            return new HttpClientMock(RootUri)
                .RegisterJsonResponse(UniqueIdentifierNameRequestUri, UniqueIdentifierNameJson)
                .RegisterJsonResponse(EntityWithQueryableMemberRequestUri, EntityWithQueryableMemberJson)
                .ToHttpClient();
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

        private static string EntityWithQueryableMemberRequestUri =>
            $"{RootUri}entitywithqueryablemembers";

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

        private static QueryResult<object> EntityWithQueryableMemberResult => new QueryResult<object>(
            new Uri(EntityWithQueryableMemberRequestUri),
            new object[]
            {
                new EntityWithQueryableMember(
                    "Test1",
                    new Query<EntityWithSimpleMembers>(CreateQueryProvider())),
                        //MockQueryProviderFactory().Create(new Uri(RootUri, "entitywithqueryablemembers/Test1/")))),
                new EntityWithQueryableMember(
                    "Test2",
                    new Query<EntityWithSimpleMembers>(CreateQueryProvider())),
                        //MockQueryProviderFactory().Create(new Uri(RootUri, "entitywithqueryablemembers/Test2/"))))
            });

        private static QueryProvider CreateQueryProvider()
        {
            return new QueryProvider(CreateResourceRetriever(), null, () => new QueryBinder(new EntityValidator()), RootUri);
        }

        private static string UniqueIdentifierNameRequestUri =>
            $"{RootUri}entitywithqueryablemembers?fields=uniqueIdentifier,name";

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
            new Uri(UniqueIdentifierNameRequestUri),
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
