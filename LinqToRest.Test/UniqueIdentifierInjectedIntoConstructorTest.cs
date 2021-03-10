using Messerli.LinqToRest.Entities;
using Xunit;

namespace Messerli.LinqToRest.Test
{
    public class UniqueIdentifierInjectedIntoConstructorTest
    {
        [Fact]
        public void UniqueIdentifierCanBeInjected()
        {
            var validator = new EntityValidator();
            validator.ValidateResourceEntity(typeof(Entity));
        }

        public sealed class Entity : IEntity
        {
            public Entity(string uniqueIdentifier)
            {
                UniqueIdentifier = uniqueIdentifier;
            }

            public string UniqueIdentifier { get; }
        }
    }
}
