using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Messerli.LinqToRest.Entities;
using Messerli.ServerCommunication;
using Messerli.Utility.Extension;
using Newtonsoft.Json.Linq;
using Soltys.ChangeCase;

namespace Messerli.LinqToRest
{
    public delegate IQueryable<object> QueryableFactory(Type type, Uri uri);

    public class ResourceRetriever : IResourceRetriever
    {
        private readonly HttpClient _httpClient;
        private readonly QueryableFactory _queryableFactory;

        public ResourceRetriever(HttpClient httpClient, QueryableFactory queryableFactory)
        {
            _httpClient = httpClient;
            _queryableFactory = queryableFactory;
        }

        public T RetrieveResource<T>(Uri uri)
        {
            var content = GetContent(uri).Result;

            return typeof(T).IsEnumerable()
                ? DeserializeArray<T>(content, uri)
                : DeserializeObject<T>(content, uri);
        }

        public object RetrieveResource(Type type, Uri uri)
        {
            var method = typeof(ResourceRetriever)
                .GetMethods()
                .First(m => m.Name == nameof(RetrieveResource))
                .MakeGenericMethod(type);

            return method.Invoke(this, new object[] { uri });
        }

        private T DeserializeObject<T>(string content, Uri uri)
        {
            var jsonObject = JObject.Parse(content);

            return (T)Deserialize(typeof(T), jsonObject, uri);
        }

        private T DeserializeArray<T>(string content, Uri uri)
        {
            var jsonArray = JArray.Parse(content);

            var type = typeof(T).GetInnerType();
            var deserialized = jsonArray.Select(token => Deserialize(type, token, uri)).ToArray();
            var castMethod = typeof(Enumerable)
                                 .GetMethod(nameof(Enumerable.Cast)) ?? throw new MissingMethodException();

            var castArray = (T)castMethod
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { deserialized });

            var toArrayMethod = typeof(Enumerable)
                                    .GetMethod(nameof(Enumerable.ToArray)) ?? throw new MissingMethodException();
            return (T)toArrayMethod
                .MakeGenericMethod(type)
                .Invoke(null, new object[] { castArray });
        }

        private object Deserialize(Type type, JToken token, Uri root)
        {
            var resourceUri = GetResourceUri(token, root);

            var constructor = type
                .GetConstructors()
                .First();

            var parameters = constructor
                .GetParameters()
                .Select(parameter => GetDeserializedParameter(parameter, token, resourceUri))
                .ToArray();

            return constructor.Invoke(parameters);
        }

        private static Uri GetResourceUri(JToken token, Uri root)
        {
            var uniqueIdentifier = GetField(token, nameof(IEntity.UniqueIdentifier));

            // Todo: Use UriBuilder once it's out of Messerli.Update
            // See <https://github.com/messerli-informatik-ag/server-communication/issues/4>
            var path = root.GetLeftPart(UriPartial.Path) + "/";
            var pathUri = new Uri(path, UriKind.Absolute);
            var resourceUri = new Uri(pathUri, uniqueIdentifier + "/");
            return resourceUri;
        }

        private object GetDeserializedParameter(ParameterInfo parameter, JToken token, Uri uri)
        {
            var type = parameter.ParameterType;
            return type.IsQueryable()
                ? _queryableFactory(type.GetInnerType(), uri)
                : GetField(token, parameter.Name);
        }

        private static object GetField(JToken token, string name)
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
