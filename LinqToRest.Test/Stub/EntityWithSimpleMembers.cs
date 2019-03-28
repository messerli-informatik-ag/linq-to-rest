using Messerli.LinqToRest.Entities;
using System;

namespace Messerli.LinqToRest.Test.Stub
{
    public class EntityWithSimpleMembers : IEntity, IEquatable<EntityWithSimpleMembers>
    {
        public EntityWithSimpleMembers(string name, int number)
        {
            Name = name;
            Number = number;
        }

        public string Name { get; }

        public int Number { get; }

        public string UniqueIdentifier => Name;

        #region manualy created equality functions

        public bool Equals(EntityWithSimpleMembers other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Number == other.Number;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EntityWithSimpleMembers)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ Number;
            }
        }

        public static bool operator ==(EntityWithSimpleMembers left, EntityWithSimpleMembers right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityWithSimpleMembers left, EntityWithSimpleMembers right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
