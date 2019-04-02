using System;
using Messerli.LinqToRest.Test.Stub;
using RichardSzalay.MockHttp;
using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class QueryProviderBuilderTest
    {
        [Fact]
        public void ThrowsOnUnconfiguredRoot()
        {
            var builder = new QueryProviderBuilder();
            Assert.Throws<QueryProviderBuilderException>(() => builder.Build<EntityWithQueryableMember>());
        }
        
        [Fact]
        public void ReturnsQueryProvider()
        {
            var builder = new QueryProviderBuilder();
            var queryable = builder
                .Root(RootStub)
                .Build<EntityWithQueryableMember>();
            
            Assert.NotNull(queryable);
        } 
        
        [Fact]
        public void HttpClientCanBeConfigured()
        {
            var builder = new QueryProviderBuilder();
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
            var builder = new QueryProviderBuilder();
            builder
                .HttpClient(null)
                .Root(RootStub);
            
            Assert.Throws<QueryProviderBuilderException>(() => builder.Build<EntityWithQueryableMember>());
        }

        [Fact]
        public void ThrowsWhenRootIsNull()
        {
            var builder = new QueryProviderBuilder();
            builder.Root(null);
            
            Assert.Throws<QueryProviderBuilderException>(() => builder.Build<EntityWithQueryableMember>());
        }
        
        private static Uri RootStub => new Uri("https://www.example.com");
    }
}
