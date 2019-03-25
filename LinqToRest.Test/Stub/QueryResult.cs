using System;
using System.Linq;

namespace Messerli.LinqToRest.Test.Stub
{
    public class QueryResult<T> : IEquatable<QueryResult<T>>
    {
        public string Query { get; }
        public T[] Object { get; }

        public QueryResult(IQueryable<T> query)
        {
            Query = query.ToString();
            Object = query.ToArray();
        }

        public QueryResult(Uri query, T[] @object)
        {
            Query = query.ToString();
            Object = @object;
        }

        #region IEquatable generated

        public bool Equals(QueryResult<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Query.Equals(other.Query) && Object.SequenceEqual(other.Object);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((QueryResult<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Query != null ? Query.GetHashCode() : 0) * 397) ^ (Object != null ? Object.GetHashCode() : 0);
            }
        }

        public static bool operator ==(QueryResult<T> left, QueryResult<T> right)
        {
            return left?.Equals(right) ?? right is null;
        }

        public static bool operator !=(QueryResult<T> left, QueryResult<T> right)
        {
            return !left?.Equals(right) ?? !(right is null);
        }

        #endregion
    }
}