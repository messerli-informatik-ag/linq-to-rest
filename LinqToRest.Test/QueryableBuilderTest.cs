using System;
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
        
        private static Uri RootStub => new Uri("https://www.example.com");
    }
}
