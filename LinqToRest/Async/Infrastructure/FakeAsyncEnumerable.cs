using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Messerli.LinqToRest.Async.Infrastructure
{
    internal class FakeAsyncEnumerable<T> : IAsyncEnumerable<T>, IEnumerable<T>
    {
        private readonly IEnumerable<T> _source;

        public FakeAsyncEnumerable(IEnumerable<T> source)
        {
            _source = source;
        }

        public async IAsyncEnumerator<T> GetAsyncEnumerator([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            foreach (var item in _source)
            {
                yield return item;
            }

            await Task.CompletedTask;
        }

        public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
