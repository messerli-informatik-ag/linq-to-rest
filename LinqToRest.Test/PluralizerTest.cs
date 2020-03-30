using Xunit;

namespace Messerli.LinqToRest.Test
{
    public sealed class PluralizerTest
    {
        [Theory]
        [MemberData(nameof(GetPluralizeData))]
        public void PluralizesCorrect(string singular, string expectedPlural)
        {
            var result = Pluralizer.Pluralize(singular);
            Assert.Equal(expectedPlural, result);
        }


        public static TheoryData<string, string> GetPluralizeData()
            => new TheoryData<string, string>
            {
                { "name", "names" },
                { "distribution", "distributions" },
                { "library", "libraries" },
                { "boy", "boys" },
                { "child", "children" },
                { "horse", "horses" },
                { "cash", "cash" },
                { "clothing", "clothing" },
                { "energy", "energy" },
                { "wildlife", "wildlife" },
                { "tornado", "tornadoes" },
                { "die", "dice" },
                { "pickaxe", "pickaxes" },
            };
    }
}
