using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Xunit;
using Distribution = Update.Client.ServerCommunication.Rest.Entities.Distribution;
using Modifier = Update.Client.ServerCommunication.Rest.Entities.Modifier;
using QueryBinderFactory = Update.Client.ServerCommunication.LinqToRest.QueryBinderFactory;

namespace LinqToRest.LinqToRest.Test
{
    public class DownloadServerTest
    {
        private const string AutofacJsonConfigurationFile = "autofac.json";

        [Theory]
        [MemberData(nameof(GetRestQueries))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public void ReturnsRestQuery(IQueryable<object> query, string result, object[] expectedQueryObject)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        {
            Assert.Equal(result, query.ToString());
        }

        [Theory]
        [MemberData(nameof(GetRestQueries))]
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public void ReturnsRestObject(IQueryable<object> query, string expectedQueryString, object[] expectedQueryObject)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        {
            var queryResult = query.ToArray();

            // Assert.Equals() calls Query<T>.GetEnumerable().Equals() and not Query<T>.Equals()
            // which executes queries :(
            expectedQueryObject
                .Zip(queryResult, (expected, actual) => new { expected, actual })
                .ForEach(obj =>
                {
                    GetPropertyValues(obj.expected)
                        .Zip(GetPropertyValues(obj.actual), (expected, actual) => new { expected, actual })
                        .ForEach(zip => AssertEquals(zip.expected, zip.actual));
                });
        }

        private static IEnumerable<object> GetPropertyValues(object obj)
        {
            return obj.GetType().GetProperties().Select(property => property.GetValue(obj));
        }

        private static void AssertEquals(object expected, object actual)
        {
            var isEqual = expected.Equals(actual);
            Assert.True(isEqual);
        }

        public static IEnumerable<object[]> GetRestQueries()
        {
            // no internal queryable
            yield return new object[]
            {
                ResolveDownloadServer().Distributions.Select(d => new {d.Name}),
                NameOfDistributionRequest,
                new object[]
                {
                    new {Name = "BauAd"},
                    new {Name = "Dispo"}
                }
            };
            yield return new object[]
            {
                ResolveDownloadServer().Distributions.Select(d => new {d.Name, d.DefaultChannel, d.License}),
                NameAndDefaultChannelAndLicenseOfDistributionRequest,
                new object[]
                {
                    new {Name = "BauAd", DefaultChannel = "Standard", License = License.Active},
                    new {Name = "Dispo", DefaultChannel = "Standard", License = License.Stale}
                }
            };

            // internal queryable
            yield return new object[]
            {
                ResolveDownloadServer().Distributions,
                DistributionRequest,
                new object[]
                {
                    new Distribution("BauAd", "Standard", License.Active,
                        new Query<Channel>(ResolveQueryProvider()), new Query<Modifier>(ResolveQueryProvider())),
                    new Distribution("Dispo", "Standard", License.Stale,
                        new Query<Channel>(ResolveQueryProvider()), new Query<Modifier>(ResolveQueryProvider()))
                }
            };
            yield return new object[]
            {
                ResolveDownloadServer().Distributions.Select(d => new {d.Name, d.Channels}),
                NameAndChannelOfDistributionRequest,
                new object[]
                {
                    new {Name = "BauAd", Channels = (IQueryable<Channel>) new Query<Channel>(ResolveQueryProvider())},
                    new {Name = "Dispo", Channels = (IQueryable<Channel>) new Query<Channel>(ResolveQueryProvider())}
                }
            };
        }

        private static IDownloadServer ResolveDownloadServer()
        {
            var compositionRoot = new CompositionRoot();
            var container = compositionRoot.Build(AutofacJsonConfigurationFile);

            return new DownloadServer(
                new QueryableFactory(
                    new ServerCommunication.LinqToRest.QueryProvider(
                        new HttpResourceRetriever(
                            container.Resolve<HttpClient>(),
                            new Deserializer(
                                new QueryableObjectCreator(container.Resolve<IQueryableFactory>()),
                                new QueryableObjectResolver(container.Resolve<IQueryableFactory>()))
                            ),
                        container.Resolve<UpdateServerUri>(),
                        container.Resolve<QueryBinderFactory>())));
        }

        private static QueryProvider.QueryProvider.QueryProvider ResolveQueryProvider()
        {
            var compositionRoot = new CompositionRoot();
            var container = compositionRoot.Build(AutofacJsonConfigurationFile);

            return container.Resolve<QueryProvider.QueryProvider.QueryProvider>();
        }

        private static Uri RootUri => new UriBuilder("update.messerli.ch")
        {
            Port = 8080,
            Scheme = "https",
            Path = "api/v1"
        }.Uri;

        private static string NameOfDistributionRequest => $"{RootUri}/distributions?fields=name";

        private static string NameAndDefaultChannelAndLicenseOfDistributionRequest => $"{RootUri}/distributions?fields=name,defaultChannel,license";

        private static string NameAndChannelOfDistributionRequest => $"{RootUri}/distributions?fields=name,channels";

        private static string DistributionRequest => $"{RootUri}/distributions";
    }
}