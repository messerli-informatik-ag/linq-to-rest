using LinqToRest.LinqToRest.Declarations;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace LinqToRest.LinqToRest
{
    internal sealed class ProjectedFields
    {
        internal ProjectedFields(Expression projector, ReadOnlyCollection<FieldDeclaration> fields)
        {
            Projector = projector;
            Fields = fields;
        }

        internal Expression Projector { get; }

        internal ReadOnlyCollection<FieldDeclaration> Fields { get; }
    }
}
