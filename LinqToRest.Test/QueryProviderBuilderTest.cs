using System;
using RichardSzalay.MockHttp;
using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class QueryProviderBuilderTest
    {
        [Fact]
        public void UnconfiguredBuilderReturnsError()
        {
            var builder = new QueryProviderBuilder();
            Assert.Throws<QueryProviderBuilderException>(() => builder.Build());
        }
        
        [Fact]
        public void ReturnsQueryProvider()
        {
            var builder = new QueryProviderBuilder();
            var queryProvider = builder
                .Root(RootStub)
                .Build();
            
            Assert.NotNull(queryProvider);
        } 
        
        [Fact]
        public void HttpClientCanBeConfigured()
        {
            var builder = new QueryProviderBuilder();
            var httpClient = new MockHttpMessageHandler().ToHttpClient();
            var queryProvider = builder
                .HttpClient(httpClient)
                .Root(RootStub)
                .Build();
            
            Assert.NotNull(queryProvider);
        }

        private static Uri RootStub => new Uri("https://www.example.com");
    }
}