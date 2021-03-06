using System;
using System.Collections;
using System.Collections.Generic;

namespace Messerli.LinqToRest
{
    public class ProjectionReader<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public ProjectionReader(IResourceRetriever resourceRetriever, Uri uri)
        {
            _enumerator = resourceRetriever.RetrieveResource<IEnumerable<T>>(uri).Result.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}