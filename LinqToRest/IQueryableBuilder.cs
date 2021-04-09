using System;
using System.Linq;
using System.Net.Http;

namespace Messerli.LinqToRest
{
    public interface IQueryableBuilder
    {
        IQueryableBuilder Root(Uri uri);

        IQueryableBuilder HttpClient(HttpClient httpClient);

        IQueryableBuilder ResourceNamingPolicy(INamingPolicy namingPolicy);

        IQueryable<T> Build<T>();
    }
}
