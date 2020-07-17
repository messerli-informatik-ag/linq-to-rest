using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messerli.LinqToRest
{
    public interface IResourceRetriever
    {
        Task<T> RetrieveResource<T>(Uri uri, CancellationToken cancellationToken = default);
    }
}
