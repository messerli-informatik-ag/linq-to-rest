using System.Linq.Expressions;
using Messerli.LinqToRest.Async;

namespace Messerli.LinqToRest
{
    internal interface IStringifyableQueryProvider : IAsyncQueryProvider
    {
        string GetQueryText(Expression expression);
    }
}
