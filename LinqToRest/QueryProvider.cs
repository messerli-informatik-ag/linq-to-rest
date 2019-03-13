using LinqToRest.LinqToRest.Expressions;
using System;
using System.Linq.Expressions;
using BaseQueryProvider = QueryProvider.QueryProvider.QueryProvider;

namespace LinqToRest.LinqToRest
{
    public delegate QueryBinder QueryBinderFactory();

    public class QueryProvider : BaseQueryProvider
    {
        private readonly IResourceRetriever _resourceRetriever;
        private readonly Uri _root;
        private readonly QueryBinderFactory _queryBinderFactory;

        public QueryProvider(IResourceRetriever resourceRetriever, UpdateServerUri root, QueryBinderFactory queryBinderFactory)
        {
            _resourceRetriever = resourceRetriever;
            _queryBinderFactory = queryBinderFactory;
            _root = root.Uri;
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

            return Activator.CreateInstance(typeof(ProjectionReader<>).MakeGenericType(elementType), _resourceRetriever, uri);
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
    }
}