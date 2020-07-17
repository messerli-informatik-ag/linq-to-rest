using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Messerli.LinqToRest.Async.Infrastructure
{
    public class FakeAsyncEnumerable<T> : IAsyncEnumerable<T>, IEnumerable<T>
    {
        private readonly IEnumerable<T> _source;

        public FakeAsyncEnumerable(IEnumerable<T> source)
        {
            _source = source;
        }

        public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var item in _source)
            {
                yield return item;
            }

            await Task.CompletedTask;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _source.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
