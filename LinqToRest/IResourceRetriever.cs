using System;
using System.Threading.Tasks;

namespace LinqToRest.LinqToRest
{
    public interface IResourceRetriever
    {
        Task<T> RetrieveResource<T>(Uri uri);
    }
}