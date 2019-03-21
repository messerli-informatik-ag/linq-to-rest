using System;
using System.Collections.Generic;
using Messerli.LinqToRest.Test.Stub;
using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using NSubstitute;
using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class QueryProviderTest
    {
        [Fact]
        public void ReturnsRestQuery()
        {
            var serviceUri = ResolveServiceUri();

            var resourceRetriever = ResolveResourceRetriever();
            var objectResolver = ResolveObjectResolver();
            var queryBinderFactory = ResolveQueryBinderFactory();


            var queryProvider = new QueryProvider(resourceRetriever, queryBinderFactory, serviceUri);
            var query = new Query<ClassWithQueryableMember>(queryProvider);

            var restQuery = query.ToString();

            Assert.Equal($"{serviceUri.AbsoluteUri}classwithqueryablemembers", restQuery);
        }

        private static Uri ResolveServiceUri()
        {
            const string root = "http://www.exapmle.com/api/v1/";
            return new UriBuilder(root).Uri;
        }

        private static IResourceRetriever ResolveResourceRetriever()
        {
            var resourceRetriever = Substitute.For<IResourceRetriever>();

            // Mock
            var uri = new Uri(ResolveServiceUri(), "test");
            var result = new[]
            {
                new ClassWithQueryableMember("Test1", null),
                new ClassWithQueryableMember("Test2", null)
            };
            resourceRetriever.RetrieveResource<IEnumerable<ClassWithQueryableMember>>(uri).Returns(result);

            return resourceRetriever;
        }

        private static IObjectResolver ResolveObjectResolver()
        {
            var queryableFactory = new QueryableFactory(ResolveQueryProviderFactory());

            return new QueryableObjectResolver(queryableFactory);
        }

        private static QueryProvider ResolveQueryProviderFactory()
        {
            return new QueryProvider(
                ResolveResourceRetriever(),
                ResolveQueryBinderFactory(),
                ResolveServiceUri());
        }

        private static QueryBinderFactory ResolveQueryBinderFactory()
        {
            var queryBinderFactory = Substitute.For<QueryBinderFactory>();
            queryBinderFactory().Returns(new QueryBinder(new EntityValidator()));

            return queryBinderFactory;
        }
    }
}