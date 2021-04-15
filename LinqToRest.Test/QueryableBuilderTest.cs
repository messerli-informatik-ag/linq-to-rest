using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Messerli.ChangeCase;
using Messerli.LinqToRest.Async;
using Messerli.LinqToRest.Entities;
using Messerli.LinqToRest.Test.Stub;
using RichardSzalay.MockHttp;
using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class QueryableBuilderTest
    {
        [Fact]
        public void ThrowsOnUnconfiguredRoot()
        {
            var builder = new QueryableBuilder();
            Assert.Throws<QueryableBuilderException>(() => builder.Build<EntityWithQueryableMember>());
        }

        [Fact]
         public void ReturnsQueryable()
        {
            var builder = new QueryableBuilder();
            var queryable = builder
                .Root(RootStub)
                .Build<EntityWithQueryableMember>();

            Assert.NotNull(queryable);
        }

        [Fact]
        public void HttpClientCanBeConfigured()
        {
            var builder = new QueryableBuilder();
            var httpClient = new MockHttpMessageHandler().ToHttpClient();
            var queryable = builder
                .HttpClient(httpClient)
                .Root(RootStub)
                .Build<EntityWithQueryableMember>();

            Assert.NotNull(queryable);
        }

        [Fact]
        public void ThrowsWhenHttpClientIsNull()
        {
            var builder = new QueryableBuilder();
            builder
                .HttpClient(null)
                .Root(RootStub);

            Assert.Throws<QueryableBuilderException>(() => builder.Build<EntityWithQueryableMember>());
        }

        [Fact]
        public void ThrowsWhenRootIsNull()
        {
            var builder = new QueryableBuilder();
            builder.Root(null);

            Assert.Throws<QueryableBuilderException>(() => builder.Build<EntityWithQueryableMember>());
        }

        [Fact]
        public async Task CustomResourceNamingPolicyIsRespected()
        {
            var httpClient = new HttpClientMockBuilder()
                .JsonResponse("/entity-with-more-than-one-words", "[{ \"uniqueIdentifier\": \"foo\", \"stringProperty\": \"bar\" }]")
                .Build();

            var queryable = new QueryableBuilder()
                .Root(RootStub)
                .HttpClient(httpClient)
                .ResourceNamingPolicy(NamingPolicy.KebabCasePlural)
                .Build<EntityWithMoreThanOneWord>();

            Assert.Single(await queryable.ToArrayAsync(), new EntityWithMoreThanOneWord("foo", "bar"));
        }

        [Fact]
        public async Task ResourceCanBeRetrievedUsingResourceRetriever()
        {
            var httpClient = new HttpClientMockBuilder()
                .JsonResponse("/custom-url", "[{ \"uniqueIdentifier\": \"foo\", \"stringProperty\": \"bar\" }]")
                .Build();

            var resourceRetriever = new QueryableBuilder()
                .Root(RootStub)
                .HttpClient(httpClient)
                .BuildResourceRetriever<EntityWithMoreThanOneWord>();

            Assert.Single(
                await resourceRetriever.RetrieveResource<IEnumerable<EntityWithMoreThanOneWord>>(new Uri($"{RootStub}custom-url")),
                new EntityWithMoreThanOneWord("foo", "bar"));
        }

        private static Uri RootStub => new Uri("https://www.example.com");
    }

    internal sealed record EntityWithMoreThanOneWord : IEntity
    {
        public EntityWithMoreThanOneWord(string uniqueIdentifier, string stringProperty)
        {
            UniqueIdentifier = uniqueIdentifier;
            StringProperty = stringProperty;
        }

        public string UniqueIdentifier { get; }

        public string StringProperty { get; }
    }
}
