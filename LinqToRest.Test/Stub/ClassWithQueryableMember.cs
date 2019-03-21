using Messerli.LinqToRest.Entities;
using System.Linq;

namespace Messerli.LinqToRest.Test.Stub
{
    public class ClassWithQueryableMember : IEntity
    {
        public ClassWithQueryableMember(string name, IQueryable<ClassWithSimpleMembers> queryableMember)
        {
            Name = name;
            QueryableMember = queryableMember;
        }

        public string Name { get; }

        public IQueryable<ClassWithSimpleMembers> QueryableMember { get; }

        public string UniqueIdentifier => Name;
    }
}
