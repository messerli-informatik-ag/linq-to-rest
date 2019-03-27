using System;

namespace Messerli.LinqToRest
{
    public interface IResourceRetriever
    {
        T RetrieveResource<T>(Uri uri);

        object RetrieveResource(Type type, Uri uri);
    }
}
