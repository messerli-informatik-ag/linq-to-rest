using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Messerli.LinqToRest.Entities;
using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using Messerli.Utility.Extension;
using Newtonsoft.Json.Linq;
using Soltys.ChangeCase;

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

        public T RetrieveResource<T>(Uri uri)
        {
            var content = GetContent(uri).Result;

            return typeof(T).IsEnumerable()
                ? DeserializeArray<T>(content)
                : DeserializeObject<T>(content);
        }

        public object RetrieveResource(Type type, Uri uri)
        {
            var method = typeof(ResourceRetriever)
                .GetMethods()
                .First(m => m.Name == nameof(RetrieveResource))
                .MakeGenericMethod(type);

            return method.Invoke(this, new object[] { uri });
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
            var deserialized = jsonArray.Select(token => Deserialize(type, token)).ToArray();
            var castMethod = typeof(Enumerable)
                                 .GetMethod(nameof(Enumerable.Cast)) ?? throw new MissingMethodException();
            return (T)castMethod
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { deserialized });
        }

        private object Deserialize(Type type, JToken token)
        {
            var uniqueIdentifier = GetField(token, nameof(IEntity.UniqueIdentifier));

            var constructor = type
                .GetConstructors()
                .First();

            var parameters = constructor
                .GetParameters()
                .Select(parameter => GetDeserializedParameter(parameter, token))
                .ToArray();

            return constructor.Invoke(parameters);
        }

        private object GetDeserializedParameter(ParameterInfo parameter, JToken token)
        {
            var type = parameter.ParameterType;
            return type.IsQueryable()
                ? _queryableFactory.CreateQueryable(type.GetInnerType()) as object
                : GetField(token, parameter.Name);
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
