using System;
using System.Net.Http;
using System.Threading.Tasks;
using Messerli.ServerCommunication;

namespace Messerli.LinqToRest
{
    public class ResourceRetriever : IResourceRetriever
    {
        public Task<T> RetrieveResource<T>(Uri uri)
        {
            throw new NotImplementedException();
        }

        public Task<object> RetrieveResource(Type type, Uri uri)
        {
            throw new NotImplementedException();
        }
    }
}
