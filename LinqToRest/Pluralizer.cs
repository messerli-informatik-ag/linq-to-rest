using System.Linq;

namespace Messerli.LinqToRest
{
    internal static class Pluralizer
    {
        public static string Pluralize(string noun)
            // Todo: search NuGet for Pluralisation
            => noun.EndsWithConsonantAndY()
                ? noun.Take(noun.Length - 1) + "ies"
                : noun + "s";

        private static bool EndsWithConsonantAndY(this string noun)
            => noun.EndsWith("y") &&
               noun.Reverse().Skip(1).First().IsConsonant();

        private static bool IsConsonant(this char letter)
            => !letter.IsVowel();

        private static bool IsVowel(this char letter)
            => "aeiou".Contains(letter);
    }
}
