using Messerli.LinqToRest.Declarations;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Messerli.LinqToRest.Expressions
{
    public class FieldExpression : Expression
    {
        internal FieldExpression(Type type, string name, IReadOnlyCollection<FieldDeclaration> properties)
        {
            Name = name;
            Properties = properties;
            Type = type;
        }

        public override System.Linq.Expressions.ExpressionType NodeType => (System.Linq.Expressions.ExpressionType)ExpressionType.Field;

        public override Type Type { get; }

        internal string Name { get; }

        internal IReadOnlyCollection<FieldDeclaration> Properties { get; }
    }
}