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
            var queryProvider = builder.Root(new Uri("https://www.example.com")).Build();
            
            Assert.NotNull(queryProvider);
        } 
        
        [Fact]
        public void HttpClientCanBeConfigured()
        {
            var builder = new QueryProviderBuilder();
            var httpClient = new MockHttpMessageHandler().ToHttpClient();
            var queryProvider = builder
                .HttpClient(httpClient)
                .Root(new Uri("https://www.example.com"))
                .Build();
            
            Assert.NotNull(queryProvider);
        }
        
    }
}