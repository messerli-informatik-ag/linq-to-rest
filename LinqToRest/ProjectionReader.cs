using System;
using System.Collections;
using System.Collections.Generic;
using Messerli.ServerCommunication;

namespace Messerli.LinqToRest
{
    public class ProjectionReader<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public ProjectionReader(IResourceRetriever resourceRetriever, IObjectResolver objectResolver, Uri uri)
        {
            _enumerator = resourceRetriever.RetrieveResource<IEnumerator<T>>(uri);
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