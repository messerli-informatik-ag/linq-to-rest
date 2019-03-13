using JetBrains.Annotations;
using LinqToRest.LinqToRest.Entities;
using System.Collections.Generic;
using Xunit;

namespace LinqToRest.LinqToRest.Test
{
    public class EntityValidatorTest
    {

        [Fact]
        public void ValidatesValidEntity()
        {
            var validator = new EntityValidator();
            validator.ValidateResourceEntity(typeof(ValidEntity));
        }

        private class ValidEntity : IEntity
        {
            public ValidEntity(string foo)
            {
                Foo = foo;
            }

            [UsedImplicitly]
            public string Foo { get; }

            public string UniqueIdentifier => Foo;
        }


        [Fact]
        public void ValidatesValidEntityWithManyProperties()
        {
            var validator = new EntityValidator();
            validator.ValidateResourceEntity(typeof(ValidEntityWithManyProperties));
        }

        private class ValidEntityWithManyProperties : IEntity
        {
            public ValidEntityWithManyProperties(string foo, int bar, IReadOnlyCollection<string> baz)
            {
                Foo = foo;
                Bar = bar;
                Baz = baz;
            }

            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }


            [UsedImplicitly]
            public IReadOnlyCollection<string> Baz { get; }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ValidatesEmptyEntity()
        {
            var validator = new EntityValidator();
            validator.ValidateResourceEntity(typeof(EmptyEntity));
        }

        private class EmptyEntity : IEntity
        {
            public string UniqueIdentifier => "";
        }


        [Fact]
        public void ValidatesEntityWithOnlyMethods()
        {
            var validator = new EntityValidator();
            validator.ValidateResourceEntity(typeof(EntityWithOnlyMethods));
        }

        private class EntityWithOnlyMethods : IEntity
        {
            [UsedImplicitly]
            public void Foo()
            {
            }

            public string UniqueIdentifier => "";
        }

        [Fact]
        public void ValidatesEntityWithOnlyStaticMethods()
        {
            var validator = new EntityValidator();
            validator.ValidateResourceEntity(typeof(EntityWithOnlyStaticMethods));
        }

        private class EntityWithOnlyStaticMethods : IEntity
        {
            [UsedImplicitly]
            public static void Foo()
            {
            }

            public string UniqueIdentifier => "";
        }

        [Fact]
        public void ValidatesEntityWithPropertiesAndMethods()
        {
            var validator = new EntityValidator();
            validator.ValidateResourceEntity(typeof(EntityWithPropertiesAndMethods));
        }

        private class EntityWithPropertiesAndMethods : IEntity
        {
            public EntityWithPropertiesAndMethods(string foo, int bar, IReadOnlyCollection<string> baz)
            {
                Foo = foo;
                Bar = bar;
                Baz = baz;
            }

            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }


            [UsedImplicitly]
            public IReadOnlyCollection<string> Baz { get; }

            [UsedImplicitly]
            public void DoFoo()
            {
            }

            [UsedImplicitly]
            public static void DoBar()
            {
            }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityWithOnlyFields()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithOnlyFields))
            );
        }

        private class EntityWithOnlyFields : IEntity
        {
            // Disable warning "Foo is never assigned"
#pragma warning disable 0649
            [UsedImplicitly]
            public string Foo;
#pragma warning restore 0649

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityWithFieldsInConstructor()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithFieldsInConstructor))
            );
        }

        private class EntityWithFieldsInConstructor : IEntity
        {
            public EntityWithFieldsInConstructor(string foo)
            {
                Foo = foo;
            }

            [UsedImplicitly]
            public string Foo;

            public string UniqueIdentifier => Foo;
        }


        [Fact]
        public void ThrowsOnEntityWithFieldsAndPropertiesInConstructor()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithFieldsAndPropertiesInConstructor))
            );
        }

        private class EntityWithFieldsAndPropertiesInConstructor : IEntity
        {
            public EntityWithFieldsAndPropertiesInConstructor(string foo, int baz)
            {
                Foo = foo;
                Baz = baz;
            }

            [UsedImplicitly]
            public string Foo;

            [UsedImplicitly]
            public int Baz { get; }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityWithSetterProperties()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithSetterProperties))
            );
        }

        private class EntityWithSetterProperties : IEntity
        {
            public EntityWithSetterProperties(string foo)
            {
                Foo = foo;
            }

            [UsedImplicitly]
            public string Foo { get; set; }

            public string UniqueIdentifier => Foo;
        }


        [Fact]
        public void ThrowsOnEntityWithPrivateSetterProperties()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithPrivateSetterProperties))
            );
        }

        private class EntityWithPrivateSetterProperties : IEntity
        {
            public EntityWithPrivateSetterProperties(string foo)
            {
                Foo = foo;
            }

            [UsedImplicitly]
            public string Foo { get; private set; }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityWithIncompleteConstructor()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithIncompleteConstructor))
            );
        }

        private class EntityWithIncompleteConstructor : IEntity
        {
            public EntityWithIncompleteConstructor(string foo)
            {
                Foo = foo;
            }

            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityWithTooLargeConstructor()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithTooLargeConstructor))
            );
        }

        private class EntityWithTooLargeConstructor : IEntity
        {
            public EntityWithTooLargeConstructor(string foo, int bar, string baz)
            {
                Foo = foo + baz;
                Bar = bar;
            }

            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityWithUnexpectedNamesInConstructor()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithUnexpectedNamesInConstructor))
            );
        }

        private class EntityWithUnexpectedNamesInConstructor : IEntity
        {
            public EntityWithUnexpectedNamesInConstructor(string foo, int baz)
            {
                Foo = foo;
                Bar = baz;
            }

            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityWithUnexpectedTypesInConstructor()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithUnexpectedTypesInConstructor))
            );
        }

        private class EntityWithUnexpectedTypesInConstructor : IEntity
        {
            public EntityWithUnexpectedTypesInConstructor(string foo, uint bar)
            {
                Foo = foo;
                Bar = (int)bar;
            }

            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityWithUnexpectedOrderInConstructor()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithUnexpectedOrderInConstructor))
            );
        }

        private class EntityWithUnexpectedOrderInConstructor : IEntity
        {
            public EntityWithUnexpectedOrderInConstructor(int bar, string foo)
            {
                Foo = foo;
                Bar = bar;
            }

            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityWithMultipleConstructors()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityWithMultipleConstructors))
            );
        }

        private class EntityWithMultipleConstructors : IEntity
        {
            public EntityWithMultipleConstructors(string foo, int bar)
            {
                Foo = foo;
                Bar = bar;
            }

            public EntityWithMultipleConstructors()
            {
                Foo = "Foo";
                Bar = 1;
            }

            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }

            public string UniqueIdentifier => Foo;
        }

        [Fact]
        public void ThrowsOnEntityThatDoesNotImplementIEntity()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityThatDoesNotImplementIEntity))
            );
        }

        private class EntityThatDoesNotImplementIEntity
        {
            public EntityThatDoesNotImplementIEntity(string foo, int bar)
            {
                Foo = foo;
                Bar = bar;
            }


            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }
        }

        [Fact]
        public void ThrowsOnEntityThatDoesNotImplementIEntityButSharesInterface()
        {
            var validator = new EntityValidator();

            Assert.Throws<MalformedResourceEntityException>
            (
                () => validator.ValidateResourceEntity(typeof(EntityThatDoesNotImplementIEntityButSharesInterface))
            );
        }

        private class EntityThatDoesNotImplementIEntityButSharesInterface
        {
            public EntityThatDoesNotImplementIEntityButSharesInterface(string foo, int bar)
            {
                Foo = foo;
                Bar = bar;
            }


            [UsedImplicitly]
            public string Foo { get; }

            [UsedImplicitly]
            public int Bar { get; }

            [UsedImplicitly]
            public string UniqueIdentifier => Foo;
        }
    }
}