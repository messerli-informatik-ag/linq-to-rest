using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Messerli.LinqToRest.Expressions;
using Messerli.QueryProvider;

namespace Messerli.LinqToRest
{
    public delegate QueryBinder QueryBinderFactory();

    public class QueryProvider : IStringifyableQueryProvider
    {
        private readonly IResourceRetriever _resourceRetriever;
        private readonly QueryBinderFactory _queryBinderFactory;
        private readonly Uri _root;

        public QueryProvider(IResourceRetriever resourceRetriever, QueryBinderFactory queryBinderFactory, Uri root)
        {
            _resourceRetriever = resourceRetriever;
            _queryBinderFactory = queryBinderFactory;
            _root = root;
        }

        public string GetQueryText(Expression expression)
        {
            return Translate(expression).CommandText;
        }

        public TResult Execute<TResult>(Expression expression) => (TResult)Execute(expression);

        public object Execute(Expression expression)
        {
            var result = Translate(expression);
            var uri = new Uri(result.CommandText);
            var elementType = TypeSystem.GetElementType(expression.Type);

            return Activator.CreateInstance(typeof(ProjectionReader<>).MakeGenericType(elementType), _resourceRetriever, uri);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            // Validate Generic Type is a Task (might need to be adjusted if we need to support ValueTasks as well)
            if (!IsGenericTaskType(typeof(TResult)))
            {
                throw new ArgumentException($"Type is expected to be a generic Task type, but was '{typeof(TResult)}') .");
            }

            var result = Translate(expression);
            var uri = new Uri(result.CommandText, _root.IsAbsoluteUri ? UriKind.Absolute : UriKind.Relative);
            var elementType = TypeSystem.GetElementType(expression.Type);
            var enumerableType = typeof(IEnumerable<>).MakeGenericType(elementType);

            var methodToExecute = typeof(ResourceRetriever).GetMethods()
                .Single(method => method.Name == nameof(ResourceRetriever.RetrieveResource) && !method.IsGenericMethod);

            return (TResult)methodToExecute.Invoke(_resourceRetriever, new object[] { enumerableType, uri, cancellationToken});
        }

        // Copied from https://github.com/messerli-informatik-ag/query-provider/blob/master/QueryProvider/QueryProvider.cs
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new Query<TElement>(this, expression);

        // Copied from https://github.com/messerli-informatik-ag/query-provider/blob/master/QueryProvider/QueryProvider.cs
        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), (object)this, (object)expression);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        private TranslateResult Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            var queryBinder = _queryBinderFactory();
            var proj = (ProjectionExpression)queryBinder.Bind(expression);
            var commandText = new QueryFormatter(_root).Format(proj.Source);
            var projector = new ProjectionBuilder().Build(proj.Projector);

            return new TranslateResult(commandText, projector);
        }

        private static bool IsGenericTaskType(Type type) => type.GetGenericTypeDefinition() == typeof(Task<>);
    }
}
