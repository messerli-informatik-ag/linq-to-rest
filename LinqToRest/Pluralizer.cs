using CorePluralizer = Pluralize.NET.Core.Pluralizer;

namespace Messerli.LinqToRest
{
    internal static class Pluralizer
    {
        private static CorePluralizer _pluralizer = new CorePluralizer();

        public static string Pluralize(string noun)
            => _pluralizer.Pluralize(noun);
    }
}
