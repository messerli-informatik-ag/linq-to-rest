using System;
using System.Linq;
using System.Net.Http;

namespace Messerli.LinqToRest
{
    public interface IQueryableBuilder
    {
        IQueryableBuilder Root(Uri uri);

        IQueryableBuilder HttpClient(HttpClient httpClient);

        IQueryable<T> Build<T>();
    }
}
