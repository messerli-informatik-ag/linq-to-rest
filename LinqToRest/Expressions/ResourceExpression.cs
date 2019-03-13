using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LinqToRest.Declarations;

namespace LinqToRest.Expressions
{
    public class ResourceExpression : Expression
    {
        internal ResourceExpression(Type type, Expression name, IReadOnlyCollection<FieldDeclaration> fields, Expression filter)
        {
            Name = name;
            Fields = fields;
            Filter = filter;
            Type = type;
        }

        public override System.Linq.Expressions.ExpressionType NodeType => (System.Linq.Expressions.ExpressionType)ExpressionType.Resource;

        public override Type Type { get; }

        internal Expression Name { get; }

        internal IReadOnlyCollection<FieldDeclaration> Fields { get; }

        internal Expression Filter { get; }
    }
}