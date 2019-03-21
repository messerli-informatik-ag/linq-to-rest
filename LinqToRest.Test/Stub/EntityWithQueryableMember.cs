using Messerli.LinqToRest.Entities;
using System.Linq;

namespace Messerli.LinqToRest.Test.Stub
{
    public class EntityWithQueryableMember : IEntity
    {
        public EntityWithQueryableMember(string name, IQueryable<EntityWithSimpleMembers> queryableMember)
        {
            Name = name;
            QueryableMember = queryableMember;
        }

        public string Name { get; }

        public IQueryable<EntityWithSimpleMembers> QueryableMember { get; }

        public string UniqueIdentifier => Name;
    }
}
