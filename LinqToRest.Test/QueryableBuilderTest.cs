using System;
using System.Linq;
using System.Threading.Tasks;
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

            var queryable = new QueryableBuilder()
                .Root(RootStub)
                .HttpClient(httpClient)
                .Build<EntityWithMoreThanOneWord>();

            Assert.Single(
                await queryable.ExecuteCustomAsync(new Uri($"{RootStub}custom-url")),
                new EntityWithMoreThanOneWord("foo", "bar"));
        }

        [Fact]
        public async Task ResourceWithQueryableCanBeRetrievedUsingResourceRetriever()
        {
            var httpClient = new HttpClientMockBuilder()
                .JsonResponse("/custom/url", "[{ \"uniqueIdentifier\": \"foo\" }]")
                .JsonResponse("/entity-with-queryables/foo/entity-with-more-than-one-words", "[{ \"uniqueIdentifier\": \"foo\", \"stringProperty\": \"bar\" }]")
                .Build();

            var query = new QueryableBuilder()
                .Root(RootStub)
                .ResourceNamingPolicy(NamingPolicy.KebabCasePlural)
                .HttpClient(httpClient)
                .Build<EntityWithQueryable>();

            var entities = await query.ExecuteCustomAsync(new Uri($"{RootStub}custom/url"));
            var entity = entities.Single();
            Assert.Equal("foo", entity.UniqueIdentifier);

            var childEntity = (await entity.Queryable.ToListAsync()).Single();
            Assert.Equal(new EntityWithMoreThanOneWord("foo", "bar"), childEntity);
        }

        private static Uri RootStub => new Uri("https://www.example.com");
    }

    internal sealed record EntityWithQueryable : IEntity
    {
        public EntityWithQueryable(string uniqueIdentifier, IQueryable<EntityWithMoreThanOneWord> queryable)
        {
            UniqueIdentifier = uniqueIdentifier;
            Queryable = queryable;
        }

        public string UniqueIdentifier { get; }

        public IQueryable<EntityWithMoreThanOneWord> Queryable { get; }
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
