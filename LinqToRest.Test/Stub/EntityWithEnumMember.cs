using System;
using Messerli.LinqToRest.Entities;

namespace Messerli.LinqToRest.Test.Stub
{
    public class EntityWithEnumMember : IEntity, IEquatable<EntityWithEnumMember>
    {
        public EntityWithEnumMember(string name, TestEnum @enum)
        {
            Name = name;
            Enum = @enum;
        }

        public string Name { get; }

        public TestEnum Enum { get; }

        public string UniqueIdentifier => Name;

        #region generated Equatable functions

        public bool Equals(EntityWithEnumMember other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Enum == other.Enum;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EntityWithEnumMember) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (int) Enum;
            }
        }

        public static bool operator ==(EntityWithEnumMember left, EntityWithEnumMember right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityWithEnumMember left, EntityWithEnumMember right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}
