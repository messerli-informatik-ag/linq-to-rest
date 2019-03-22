using Messerli.LinqToRest.Test.Stub;
using Messerli.QueryProvider;
using Messerli.ServerCommunication;
using Messerli.Utility.Extension;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using RestQueryProvider = Messerli.LinqToRest.QueryProvider;

namespace Messerli.LinqToRest.Test
{
    public class QueryProviderTest
    {
        [Fact]
        public void ReturnsRestQuery()
        {
            var query = CreateQuery<EntityWithQueryableMember>();
            var restQuery = query.ToString();
            var expectedRestQuery = EntityWithQueryableMemberRequest.AbsoluteUri;

            Assert.Equal(expectedRestQuery, restQuery);
        }


        [Fact]
        public void ReturnsRestObject()
        {
            var query = CreateQuery<EntityWithQueryableMember>();
            var queryResult = query.ToArray();

            var expectedQueryObject = EntityWithQueryableMemberResult;

            // Assert.Equals() calls Query<T>.GetEnumerable().Equals() and not Query<T>.Equals()
            // which executes queries :(
            Assert.Equal(expectedQueryObject.Length, queryResult.Length);
            expectedQueryObject
                .Zip(queryResult, (expected, actual) => new { expected, actual })
                .ForEach(obj =>
                {
                    obj.expected.GetType().GetPropertyValues(obj.expected)
                        .Zip(obj.actual.GetType().GetPropertyValues(obj.actual),
                            (expected, actual) => new { expected, actual })
                        .ForEach(zip => AssertEquals(zip.expected, zip.actual));
                });
        }

        #region Helper

        private static void AssertEquals(object expected, object actual)
        {
            var isEqual = expected.Equals(actual);
            Assert.True(isEqual);
        }

        private static Query<T> CreateQuery<T>()
        {
            var serviceUri = MockServiceUri();
            var resourceRetriever = MockResourceRetriever();
            var objectResolver = MockObjectResolver();
            var queryBinderFactory = MockQueryBinderFactory();

            var queryProvider = new RestQueryProvider(resourceRetriever, objectResolver, queryBinderFactory, serviceUri);

            return new Query<T>(queryProvider);
        }

        #endregion

        #region Mock

        private static Uri MockServiceUri()
        {
            return RootUri;
        }

        private static IResourceRetriever MockResourceRetriever()
        {
            var retriever = Substitute.For<IResourceRetriever>();

            retriever = AddUriMock<IEnumerable<EntityWithQueryableMember>>(retriever, EntityWithQueryableMemberRequest, ResourceRetrieverEntityWithQueryableMemberResult);

            return retriever;
        }

        private static IResourceRetriever AddUriMock<T>(IResourceRetriever resourceRetriever, Uri uri, object value)
        {
            resourceRetriever.RetrieveResource<T>(uri).Returns(Task.FromResult((T)value));

            return resourceRetriever;
        }

        private static IObjectResolver MockObjectResolver()
        {
            var queryableFactory = new QueryableFactory(MockQueryProviderFactory());

            return new QueryableObjectResolver(queryableFactory);
        }

        private static QueryProviderFactory MockQueryProviderFactory()
        {
            return new QueryProviderFactory(
                MockResourceRetriever(),
                // Todo: resolve circular dependency!
                new DefaultObjectResolver(),
                MockQueryBinderFactory(),
                MockServiceUri());
        }

        private static QueryBinderFactory MockQueryBinderFactory()
        {
            var queryBinderFactory = Substitute.For<QueryBinderFactory>();
            queryBinderFactory().Returns(new QueryBinder(new EntityValidator()));

            return queryBinderFactory;
        }

        #endregion

        #region Data

        private static Uri RootUri => new Uri("http://www.exapmle.com/api/v1/", UriKind.Absolute);

        private static Uri EntityWithQueryableMemberRequest => new Uri(RootUri, "entitywithqueryablemembers");

        private static Uri EntityWithQueryableMemberTest1Root => new Uri(RootUri, "entitywithqueryablemembers/Test1/");

        private static Uri EntityWithQueryableMemberTest2Root => new Uri(RootUri, "entitywithqueryablemembers/Test2/");

        private static object ResourceRetrieverEntityWithQueryableMemberResult => new[]
        {
            new EntityWithQueryableMember("Test1", null),
            new EntityWithQueryableMember("Test2", null)
        };

        private static EntityWithQueryableMember[] EntityWithQueryableMemberResult => new[]
        {
            new EntityWithQueryableMember("Test1", new Query<EntityWithSimpleMembers>(MockQueryProviderFactory().Create(EntityWithQueryableMemberTest1Root))),
            new EntityWithQueryableMember("Test2", new Query<EntityWithSimpleMembers>(MockQueryProviderFactory().Create(EntityWithQueryableMemberTest2Root)))
        };

        #endregion
    }
}
