using Messerli.LinqToRest.Entities;
using System;
using System.Linq;

namespace Messerli.LinqToRest.Test.Stub
{
    public class EntityWithQueryableMember : IEntity, IEquatable<EntityWithQueryableMember>
    {
        public EntityWithQueryableMember(string name, IQueryable<EntityWithSimpleMembers> queryableMember)
        {
            Name = name;
            QueryableMember = queryableMember;
        }

        public string Name { get; }

        public IQueryable<EntityWithSimpleMembers> QueryableMember { get; }

        public string UniqueIdentifier => Name;

        #region manualy created equality functions

        public bool Equals(EntityWithQueryableMember other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Equals(QueryableMember, other.QueryableMember);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EntityWithQueryableMember)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (QueryableMember != null ? QueryableMember.GetHashCode() : 0);
            }
        }

        public static bool operator ==(EntityWithQueryableMember left, EntityWithQueryableMember right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityWithQueryableMember left, EntityWithQueryableMember right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
