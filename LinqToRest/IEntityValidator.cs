using System;

namespace LinqToRest.LinqToRest
{
    public interface IEntityValidator
    {
        void ValidateResourceEntity(Type type);
    }
}