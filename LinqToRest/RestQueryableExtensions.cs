using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Messerli.LinqToRest
{
    public static class RestQueryableExtensions
    {
        /// <summary>Executes a custom request. Note that any sub-entities are fetched using the default URL naming scheme.</summary>
        /// <exception cref="InvalidOperationException">Thrown when the <paramref name="queryable"/> is not from <c>LinqToRest</c>.</exception>
        public static async Task<IEnumerable<TResult>> ExecuteCustomAsync<TResult>(this IQueryable<TResult> queryable, Uri customRequestUri, CancellationToken cancellationToken = default)
            => queryable.Provider is IRestQueryProvider queryableProvider
                ? (IEnumerable<TResult>)await queryableProvider.ExecuteAsync<Task<object>>(queryable.Expression, customRequestUri, cancellationToken)
                : throw new InvalidOperationException($"The source IQueryable doesn't implement IRestQueryProvider<{typeof(TResult)}>.");
    }
}
