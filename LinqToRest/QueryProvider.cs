using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Messerli.LinqToRest.Async;
using Messerli.LinqToRest.Expressions;
using Messerli.QueryProvider;

namespace Messerli.LinqToRest
{
    public delegate QueryBinder QueryBinderFactory();

    public class QueryProvider : Messerli.QueryProvider.QueryProvider, IAsyncQueryProvider
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

        public override string GetQueryText(Expression expression)
        {
            return Translate(expression).CommandText;
        }

        public override object Execute(Expression expression)
        {
            var result = Translate(expression);
            var uri = new Uri(result.CommandText);
            var elementType = TypeSystem.GetElementType(expression.Type);

            return Activator.CreateInstance(typeof(ProjectionReader<>).MakeGenericType(elementType), _resourceRetriever,
                uri);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            // Validate Generic Type is a Task (might need to be adjusted if we need to support ValueTasks as well)
            if (!(typeof(TResult).GetGenericTypeDefinition() == typeof(Task<>)))
            {
                throw new ArgumentException("Type is expected to be a generic Task type.");
            }

            var result = Translate(expression);
            var uri = new Uri(result.CommandText);
            var elementType = TypeSystem.GetElementType(expression.Type);

            var methodToExecute = typeof(ResourceRetriever).GetMethods()
                .Single(method => method.Name == nameof(ResourceRetriever.RetrieveResource) && method.IsGenericMethod)
                .MakeGenericMethod(elementType);

            return (TResult) methodToExecute.Invoke(_resourceRetriever, new object[] {uri, cancellationToken});
        }

        private TranslateResult Translate(Expression expression)
        {
            expression = Evaluator.PartialEval(expression);
            var queryBinder = _queryBinderFactory();
            var proj = (ProjectionExpression) queryBinder.Bind(expression);
            var commandText = new QueryFormatter(_root).Format(proj.Source);
            var projector = new ProjectionBuilder().Build(proj.Projector);

            return new TranslateResult(commandText, projector);
        }
    }
}
