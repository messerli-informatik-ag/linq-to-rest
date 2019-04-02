using System;
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
    }
}