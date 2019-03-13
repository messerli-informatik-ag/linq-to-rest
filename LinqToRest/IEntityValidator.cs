using System;

namespace LinqToRest
{
    public interface IEntityValidator
    {
        void ValidateResourceEntity(Type type);
    }
}