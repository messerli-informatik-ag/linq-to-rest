using System;
using Messerli.LinqToRest.Entities;

namespace Messerli.LinqToRest.Test.Stub
{
    public class EntityWithUriMember : IEntity, IEquatable<EntityWithUriMember>
    {
        public EntityWithUriMember(string name, Uri uri)
        {
            Name = name;
            Uri = uri;
        }

        public string Name { get; }
        
        public Uri Uri { get; }

        public string UniqueIdentifier => Name;

        #region generated Equatable functions

        public bool Equals(EntityWithUriMember other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name) && Uri.Equals(other.Uri);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((EntityWithUriMember) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode() * 397) ^ Uri.GetHashCode();
            }
        }
        
        public static bool operator ==(EntityWithUriMember left, EntityWithUriMember right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityWithUriMember left, EntityWithUriMember right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}