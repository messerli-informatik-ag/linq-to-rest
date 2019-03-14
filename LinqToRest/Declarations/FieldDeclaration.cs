using System.Linq.Expressions;

namespace Messerli.LinqToRest.Declarations
{
    internal class FieldDeclaration
    {
        internal FieldDeclaration(string name, Expression expression)
        {
            Name = name;
            Expression = expression;
        }

        internal string Name { get; }

        internal Expression Expression { get; }
    }
}
