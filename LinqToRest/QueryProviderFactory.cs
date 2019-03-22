using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using System;
using System.Linq;
using System.Net;
using Messerli.LinqToRest.Entities;
using Messerli.Utility.Extension;

namespace Messerli.LinqToRest
{
    public class QueryProviderFactory : IQueryProviderFactory
    {
        private readonly IResourceRetriever _resourceRetriever;
        private readonly IObjectResolver _objectResolver;
        private readonly QueryBinderFactory _queryBinderFactory;
        private readonly Uri _root;

        public QueryProviderFactory(IResourceRetriever resourceRetriever, IObjectResolver objectResolver, QueryBinderFactory queryBinderFactory, Uri root)
        {
            _resourceRetriever = resourceRetriever;
            _objectResolver = objectResolver;
            _queryBinderFactory = queryBinderFactory;
            _root = root;
        }

        public Messerli.QueryProvider.QueryProvider Create()
        {
            return Create(_root);
        }

        public Messerli.QueryProvider.QueryProvider Create(ObjectToResolve objectToResolve)
        {
            var uriString = CreateNestedPath(objectToResolve);
            var uri = new Uri(uriString, UriKind.Absolute);

            return Create(uri);
        }

        public Messerli.QueryProvider.QueryProvider Create(Uri uri)
        {
            return new QueryProvider(_resourceRetriever, _objectResolver, _queryBinderFactory, uri);
        }

        private static string CreateNestedPath(ObjectToResolve objectToResolve)
        {
            if (objectToResolve is null)
            {
                return string.Empty;
            }

            var path = CreateNestedPath(objectToResolve.Parent);

            if (objectToResolve.Type.IsAnonymous())
            {
                return HandleAnonymous(path, objectToResolve);
            }

            switch (objectToResolve.Current)
            {
                case IEntity entity:
                    return $"{path}{entity.UniqueIdentifier}/";
                case Uri uri:
                    return $"{uri.AbsoluteUri}/";
            }

            return path;
        }

        private static string HandleAnonymous(string path, ObjectToResolve objectToResolve)
        {
            var uniqueIdentifier =
                objectToResolve.Type
                    .GetProperty(nameof(IEntity.UniqueIdentifier))?
                    .GetValue(objectToResolve.Current);

            return uniqueIdentifier is null
                ? throw new ArgumentException(
                    $"{nameof(objectToResolve.Type)} does not contain essential property {nameof(IEntity.UniqueIdentifier)}")
                : $"{path}{uniqueIdentifier}/";
        }
    }
}
