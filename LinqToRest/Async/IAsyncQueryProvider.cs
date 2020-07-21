// from https://github.com/dotnet/efcore/blob/master/src/EFCore/Query/IAsyncQueryProvider.cs

// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using JetBrains.Annotations;

namespace Messerli.LinqToRest.Async
{
    /// <summary>
    ///     <para>
    ///         Defines method to execute queries asynchronously that are described by an IQueryable object.
    ///     </para>
    ///     <para>
    ///         The implementation does not need to be thread-safe.
    ///     </para>
    /// </summary>
    internal interface IAsyncQueryProvider : IQueryProvider
    {
        /// <summary>
        ///     Executes the strongly-typed query represented by a specified expression tree asynchronously.
        /// </summary>
        TResult ExecuteAsync<TResult>([NotNull] Expression expression, CancellationToken cancellationToken = default);
    }
}
