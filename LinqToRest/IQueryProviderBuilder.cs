using System;
using System.Linq;
using System.Net.Http;

namespace Messerli.LinqToRest
{
    public interface IQueryProviderBuilder
    {
        IQueryProviderBuilder Root(Uri uri);

        IQueryProviderBuilder HttpClient(HttpClient httpClient);

        IQueryable<T> Build<T>();
    }
}
