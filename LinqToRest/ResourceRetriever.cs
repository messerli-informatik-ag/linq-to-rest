using Messerli.LinqToRest.Entities;
using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using Messerli.Utility.Extension;
using Newtonsoft.Json.Linq;
using Soltys.ChangeCase;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Messerli.LinqToRest
{
    public class ResourceRetriever : IResourceRetriever
    {
        private readonly HttpClient _httpClient;
        private readonly IQueryableFactory _queryableFactory;

        public ResourceRetriever(HttpClient httpClient, IQueryableFactory queryableFactory)
        {
            _httpClient = httpClient;
            _queryableFactory = queryableFactory;
        }

        public async Task<T> RetrieveResource<T>(Uri uri)
        {
            var content = await GetContent(uri);

            return typeof(T).IsEnumerable()
                ? DeserializeArray<T>(content)
                : DeserializeObject<T>(content);
        }

        public Task<object> RetrieveResource(Type type, Uri uri)
        {
            var method = typeof(ResourceRetriever).GetMethod(nameof(RetrieveResource))?.MakeGenericMethod(type)
                         ?? throw new NullReferenceException();

            return (Task<object>)method.Invoke(this, new object[] { uri });
        }

        private T DeserializeObject<T>(string content)
        {
            var jsonObject = JObject.Parse(content);

            return (T)Deserialize(typeof(T), jsonObject);
        }

        private T DeserializeArray<T>(string content)
        {
            var jsonArray = JArray.Parse(content);

            var type = typeof(T).GetInnerType();

            return (T)jsonArray.Select(token => Deserialize(type, token));
        }

        private object Deserialize(Type type, JToken token)
        {
            var uniqueIdentifier = GetField(token, nameof(IEntity.UniqueIdentifier));

            var constructor = type
                .GetConstructors()
                .First();

            var parameters = constructor
                .GetParameters()
                .Select(p => p.GetType().IsQueryable()
                    ? _queryableFactory.CreateQueryable(p.GetType()) as object
                    : GetField(token, p.Name) as object)
                .ToArray();

            return constructor.Invoke(parameters);
        }

        private static string GetField(JToken token, string name)
        {
            var fieldName = name.CamelCase();

            return (string)token[fieldName]
                ?? throw new InvalidDataException($"The field {fieldName} is missing in the JSON response.");
        }

        private async Task<string> GetContent(Uri uri)
        {
            var distributionsResponse = await _httpClient.GetAsync(uri);

            try
            {
                distributionsResponse.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new UnavailableResourceException(uri.ToString(), e);
            }

            return await distributionsResponse.Content.ReadAsStringAsync();
        }
    }
}
