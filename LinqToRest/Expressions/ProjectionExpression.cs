using System;
using System.Linq.Expressions;

namespace Messerli.LinqToRest.Expressions
{
    public class ProjectionExpression : Expression
    {
        internal ProjectionExpression(Expression source, Expression projector)
        {
            Type = projector.Type;
            Source = source;
            Projector = projector;
        }

        public override System.Linq.Expressions.ExpressionType NodeType => (System.Linq.Expressions.ExpressionType)ExpressionType.Projection;

        public override Type Type { get; }

        internal Expression Source { get; }

        internal Expression Projector { get; }
    }
}