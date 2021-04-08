using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using Messerli.LinqToRest.Test.Stub;
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

            var uri = new Uri(NameResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { Name = default(string) }.GetType());
            var actual = await resourceRetriever.RetrieveResource(type, uri);

            Assert.Equal(NameResult.Object, actual);
        }

        [Fact]
        public async void ReturnsFullRestObjectWithSelect()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(NameNumberResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { Name = default(string), Number = default(int) }.GetType());
            var actual = await resourceRetriever.RetrieveResource(type, uri);

            Assert.Equal(NameNumberResult.Object, actual);
        }

        [Fact]
        public async void ReturnsRestObjectWithSelectedQueryable()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(NameQueryableMemberResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { Name = default(string), QueryableMember = default(IQueryable<EntityWithSimpleMembers>) }.GetType());
            var actual = await resourceRetriever.RetrieveResource(type, uri);

            Assert.Equal(NameQueryableMemberResult.Object, actual);
        }

        [Fact]
        public async void ReturnsRestObjectWithSelectedUniqueIdentifier()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(UniqueIdentifierNameResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { UniqueIdentifier = default(string), Name = default(string) }.GetType());

            var actual = await resourceRetriever.RetrieveResource(type, uri);
            var expected = UniqueIdentifierNameResult.Object;

            Assert.Equal(expected, actual);
        }


        [Fact]
        public async void ReturnsRestObjectWithEnum()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(EnumResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { Enum = default(TestEnum) }.GetType());

            var actual = await resourceRetriever.RetrieveResource(type, uri);
            var expected = EnumResult.Object;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void ReturnsRestObjectWithDateString()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(DateStringResult.Query, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { Date = default(StringNewType) }.GetType());

            var actual = await resourceRetriever.RetrieveResource(type, uri);
            var expected = DateStringResult.Object;

            Assert.Equal(expected[0], ((IEnumerable<object>)actual).First());
        }

        [Fact]
        public async System.Threading.Tasks.Task ThrowOnInvalidEnumValue()
        {
            var resourceRetriever = CreateResourceRetriever();

            var uri = new Uri(InvalidEnumRequestUri, UriKind.Absolute);
            var type = typeof(IEnumerable<>).MakeGenericType(new { Enum = TestEnum.One }.GetType());

            await Assert.ThrowsAsync<InvalidEnumArgumentException>(() => resourceRetriever.RetrieveResource(type, uri));
        }

        private static ResourceRetriever CreateResourceRetriever()
        {
            return new ResourceRetriever(MockHttpClient())
            {
                QueryableFactory = (type, uri) =>
                {
                    var queryProvider = new QueryProvider(
                        Substitute.For<IResourceRetriever>(),
                        () => new QueryBinder(new EntityValidator()),
                        uri);

                    return Activator.CreateInstance(typeof(Query<>).MakeGenericType(type), queryProvider) as IQueryable<object>;
                },
            };
        }

        #region Mock

        private static HttpClient MockHttpClient()
        {
            return new HttpClientMockBuilder()
                .JsonResponse(UniqueIdentifierNameNumberRequestUri, UniqueIdentifierNameNumberJson)
                .JsonResponse(EntityWithQueryableMemberRequestUri, EntityWithQueryableMemberJson)
                .JsonResponse(EnumRequestUri, EntityWithEnumMemberJson)
                .JsonResponse(DateRequestUri, EntityWithDateStringJson)
                .JsonResponse(InvalidEnumRequestUri, InvalidEnumJson)
                .Build();
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

        private static string UniqueIdentifierNameNumberJson => @"
[
    {
        ""uniqueIdentifier"": ""Test1"",
        ""name"": ""Test1"",
        ""number"": ""1""
    },
    {
        ""uniqueIdentifier"": ""Test2"",
        ""name"": ""Test2"",
        ""number"": ""2""
    }
]
";

        private static QueryResult<object> EntityWithQueryableMemberResult => new QueryResult<object>(
            new Uri(EntityWithQueryableMemberRequestUri),
            new object[]
            {
                new EntityWithQueryableMember("Test1", CreateQuery<EntityWithSimpleMembers>("entitywithqueryablemembers/Test1/")),
                new EntityWithQueryableMember("Test2", CreateQuery<EntityWithSimpleMembers>("entitywithqueryablemembers/Test2/"))
            });

        private static QueryProvider CreateQueryProvider(Uri root)
        {
            return new QueryProvider(CreateResourceRetriever(), () => new QueryBinder(new EntityValidator()), root);
        }

        private static IQueryable<T> CreateQuery<T>(string subPath)
        {
            return new Query<T>(CreateQueryProvider(new Uri(RootUri, subPath)));
        }

        private static string UniqueIdentifierNameRequestUri =>
            $"{RootUri}entitywithqueryablemembers?fields=uniqueIdentifier,name";

        private static QueryResult<object> NameResult => new QueryResult<object>(
            new Uri(UniqueIdentifierNameRequestUri),
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


        private static string UniqueIdentifierNameNumberRequestUri =>
            $"{RootUri}entitywithqueryablemembers?fields=uniqueIdentifier,name,number";

        private static QueryResult<object> NameNumberResult => new(
            new Uri(UniqueIdentifierNameNumberRequestUri),
            new object[]
            {
                new
                {
                    Name = "Test1",
                    Number = 1
                },
                new
                {
                    Name = "Test2",
                    Number = 2
                }
            });

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

        private static string NameQueryableMemberRequestUri =>
            $"{RootUri}entitywithqueryablemembers?fields=uniqueIdentifier,name,queryableMember";

        private static QueryResult<object> NameQueryableMemberResult => new QueryResult<object>(
            new Uri(NameQueryableMemberRequestUri),
            new object[]
            {
                new
                {
                    Name = "Test1",
                    QueryableMember = CreateQuery<EntityWithSimpleMembers>("entitywithqueryablemembers/Test1/")
                },
                new
                {
                    Name = "Test2",
                    QueryableMember = CreateQuery<EntityWithSimpleMembers>("entitywithqueryablemembers/Test2/")
                }
            });

        private static string EnumRequestUri => $"{RootUri}entitywithenummembers";

        private static string EntityWithEnumMemberJson => @"
[
    {
        ""uniqueIdentifier"": ""One"",
        ""enum"": ""One""
    },
    {
        ""uniqueIdentifier"": ""Two"",
        ""enum"": ""Two""
    },
    {
        ""uniqueIdentifier"": ""Three"",
        ""enum"": ""Three""
    },
]
";

        private static QueryResult<object> EnumResult => new QueryResult<object>(
            new Uri(EnumRequestUri),
            new object[]
            {
                new { Enum = TestEnum.One },
                new { Enum = TestEnum.Two },
                new { Enum = TestEnum.Three }
            });

        private static string DateRequestUri => $"{RootUri}entitywithdatemembers";

        private static string EntityWithDateStringJson => @"
[
    {
        ""uniqueIdentifier"": ""One"",
        ""date"": ""2021-01-03T09:45:12""
    },
]
";

        private static QueryResult<object> DateStringResult => new(
            new Uri(DateRequestUri),
            new object[]
            {
                new { Date = new StringNewType("2021-01-03T09:45:12") },
            });

        private static string InvalidEnumRequestUri => $"{RootUri}invaludenummembers";

        private static string InvalidEnumJson => @"
[
    {
        ""uniqueIdentifier"": ""One"",
        ""enum"": ""NoEnumValue""
    }
]
";

        #endregion

        private sealed record StringNewType
        {
            public StringNewType(string value)
            {
                Value = value;
            }

            public string Value { get; }

            public override string ToString() => $"{nameof(StringNewType)}({Value})";
        }
    }
}
