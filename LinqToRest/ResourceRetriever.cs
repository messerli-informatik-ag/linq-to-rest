using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

        [CanBeNull]
        public QueryableFactory QueryableFactory { get; set; }

        public ResourceRetriever(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<T> RetrieveResource<T>(Uri uri, CancellationToken cancellationToken = default)
        {
            var content = await GetContent(uri, cancellationToken).ConfigureAwait(false);

            return typeof(T).IsEnumerable()
                ? DeserializeArray<T>(content, uri)
                : DeserializeObject<T>(content, uri);
        }

        public async Task<object> RetrieveResource(Type type, Uri uri, CancellationToken cancellationToken = default)
        {
            var method = typeof(ResourceRetriever)
                .GetMethods()
                .First(m => m.Name == nameof(RetrieveResource))
                .MakeGenericMethod(type);

            var task = (Task)method.Invoke(this, new object[] { uri, cancellationToken });
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result)) ??
                                 throw new MissingMemberException();

            return resultProperty.GetValue(task);
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
            var uniqueIdentifier = GetField(typeof(string), token, nameof(IEntity.UniqueIdentifier));

            if (root.IsAbsoluteUri)
            {
                var path = root.GetLeftPart(UriPartial.Path) + "/";
                var pathUri = new Uri(path, UriKind.Absolute);
                return new Uri(pathUri, uniqueIdentifier + "/");
            }

            static string CombineUrl(string uri1, string uri2)
                => $"{uri1.TrimEnd('/')}/{uri2.TrimStart('/')}";

            return new Uri(CombineUrl(root.ToString(), uniqueIdentifier + "/"), UriKind.Relative);
        }

        private object GetDeserializedParameter(ParameterInfo parameter, JToken token, Uri uri)
        {
            var type = parameter.ParameterType;

            return type.IsQueryable()
                ? CreateQueryable(uri, type)
                : type.IsEnum
                    ? GetEnum(type, token, parameter.Name)
                    : GetField(type, token, parameter.Name);
        }

        private IQueryable<object> CreateQueryable(Uri root, Type type)
        {
            var factory = QueryableFactory ?? throw new NullReferenceException(nameof(QueryableFactory));
            return factory(type.GetInnerType(), root);
        }

        private static object GetEnum(Type type, JToken token, string name)
        {
            var candidate = GetField(typeof(string), token, name) as string
                ?? throw new ArgumentException($"Property '{nameof(name)}' was not found in json!");

            return candidate.ParseToEnumElement(type);
        }

        private static object GetField(Type type, JToken token, string name)
        {
            var fieldName = name.CamelCase();

            if (TryGetValue(token, type, fieldName, out var fieldValue))
            {
                return fieldValue;
            }

            var constructors = type
                .GetConstructors()
                .Where(constructor => constructor.GetParameters().Length == 1);

            foreach (var constructor in constructors)
            {
                if (TryGetValue(token, constructor.GetParameters().First().ParameterType, fieldName, out var parameterValue))
                {
                    return constructor.Invoke(new[] { parameterValue });
                }
            }

            throw new ArgumentException($"{type.Name} cannot be constructed from ${token[fieldName]}");
        }

        private static bool TryGetValue(JToken token, Type type, string fieldName, out object value)
        {
            var valueMethod = typeof(JToken).GetMethod(nameof(JToken.Value)) ?? throw new MissingMethodException();
            try
            {
                value = valueMethod.MakeGenericMethod(type).Invoke(token, new object[] { fieldName });
                return true;
            }
            catch (TargetInvocationException)
            {
                value = default;
                return false;
            }
        }

        private async Task<string> GetContent(Uri uri, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);

            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                throw new UnavailableResourceException(uri.ToString(), e);
            }

            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
