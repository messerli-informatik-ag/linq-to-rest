using Messerli.LinqToRest.Test.Stub;
using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class ResourceRetrieverTest
    {
        [Fact]
        public void ReturnsRestObject()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(EntityWithQueryableMemberResult.Query, UriKind.Absolute);
            var actual = resourceRetriever.RetrieveResource<IEnumerable<EntityWithQueryableMember>>(uri).Result;

            Assert.Equal(EntityWithQueryableMemberResult.Object, actual);
        }

        [Fact]
        public void ReturnsRestObjectWithSelect()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(UniqueIdentifierNameResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { Name = string.Empty }.GetType());
            var actual = resourceRetriever.RetrieveResource(type, uri).Result;

            Assert.Equal(UniqueIdentifierNameResult.Object, actual);
        }

        [Fact]
        public void ReturnsRestObjectWithSelectedUniqueIdentifier()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(UniqueIdentifierNameResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { UniqueIdentifyer = string.Empty, Name = string.Empty }.GetType());
            var actual = resourceRetriever.RetrieveResource(type, uri).Result;

            Assert.Equal(UniqueIdentifierNameResult.Object, actual);
        }

        private static ResourceRetriever CreateResourceRetriever()
        {
            return new ResourceRetriever();
        }

        #region Mock

        private static Uri MockServiceUri()
        {
            return RootUri;
        }

        private static HttpClient MockHttpClient()
        {
            return new HttpClientMock(RootUri)
                .RegisterJsonResponse(UniqueIdentifierNameRequestUri, UniqueIdentifierNameJson)
                .RegisterJsonResponse(EntityWithQueryableMemberRequestUri, EntityWithQueryableMemberJson)
                .ToHttpClient();
        }

        private static IObjectResolver MockObjectResolver()
        {
            var queryableFactory = new QueryableFactory(MockQueryProviderFactory());

            return new QueryableObjectResolver(queryableFactory);
        }

        private static QueryProviderFactory MockQueryProviderFactory()
        {
            return new QueryProviderFactory(
                CreateResourceRetriever(),
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

        private static Uri RootUri => new Uri("http://www.example.com/api/v1/", UriKind.Absolute);

        private static string EntityWithQueryableMemberRequestUri => "entitywithqueryablemembers";

        private static string EntityWithQueryableMemberJson => @"
[
    {
        ""name"": ""Test1""
    },
    {
        ""name"": ""Test2""
    }
]
";

        private static QueryResult<object> EntityWithQueryableMemberResult => new QueryResult<object>(
            new Uri(RootUri, EntityWithQueryableMemberRequestUri),
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

        private static string UniqueIdentifierNameRequestUri =>
            "entitywithqueryablemembers?fields=uniqueIdentifier,name";

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
            new Uri(RootUri, UniqueIdentifierNameRequestUri),
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
