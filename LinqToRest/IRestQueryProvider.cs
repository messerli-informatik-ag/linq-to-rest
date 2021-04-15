using System;
using System.Linq.Expressions;
using System.Threading;
using JetBrains.Annotations;
using Messerli.LinqToRest.Async;

namespace Messerli.LinqToRest
{
    internal interface IRestQueryProvider : IAsyncQueryProvider
    {
        TResult ExecuteAsync<TResult>(Expression expression, [CanBeNull] Uri customRequestUri = null, CancellationToken cancellationToken = default);
    }
}
