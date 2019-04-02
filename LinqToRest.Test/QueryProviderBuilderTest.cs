using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class QueryProviderBuilderTest
    {
        [Fact]
        public void BuildsQueryProvider()
        {
            var builder = new QueryProviderBuilder();
            builder.Build();
        }
    }
}