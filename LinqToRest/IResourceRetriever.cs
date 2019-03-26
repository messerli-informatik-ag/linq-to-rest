using System;
using System.Threading.Tasks;

namespace Messerli.LinqToRest
{
    public interface IResourceRetriever
    {
        Task<T> RetrieveResource<T>(Uri uri);

        Task<object> RetrieveResource(Type type, Uri uri);
    }
}