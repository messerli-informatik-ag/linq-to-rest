using System;

namespace Messerli.LinqToRest
{
    public interface IEntityValidator
    {
        void ValidateResourceEntity(Type type);
    }
}