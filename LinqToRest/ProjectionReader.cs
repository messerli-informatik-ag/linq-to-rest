using Messerli.ServerCommunication;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Messerli.LinqToRest
{
    public class ProjectionReader<T> : IEnumerable<T>
    {
        private readonly IEnumerator<T> _enumerator;

        public ProjectionReader(IResourceRetriever resourceRetriever, IObjectResolver objectResolver, Uri uri)
        {
            var deserializedResource = resourceRetriever.RetrieveResource<IEnumerable<T>>(uri).Result;

            var parent = new ObjectToResolve(typeof(Uri), uri, null);
            var objectToResolve = new ObjectToResolve(typeof(IEnumerable<T>), deserializedResource, parent);

            var resolvedResource = (IEnumerable<T>)objectResolver.Resolve(objectToResolve);

            // Todo: throw on null!

            _enumerator = resolvedResource.GetEnumerator();
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