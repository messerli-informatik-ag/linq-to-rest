using System.Threading.Tasks;
using Messerli.LinqToRest.Async;
using Messerli.LinqToRest.Test.Stub;
using Xunit;
using static Messerli.LinqToRest.Test.QueryProviderTestUtility;

namespace Messerli.LinqToRest.Test
{
    public class AsyncQueryProviderTest
    {
        [Fact]
        public async Task TestAsyncQueryProviderToListAsyncDoesNotCrash()
        {
            var query = CreateQuery<EntityWithUriMember>();
            // The following call is the same as ToListAsync, but might create a deadlock with a blocking .Result call.
            // var test = query.ToList();
            var result = await query.ToListAsync();
            Assert.NotNull(result);
        }
    }
}
