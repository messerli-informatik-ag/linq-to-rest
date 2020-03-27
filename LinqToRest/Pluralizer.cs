namespace Messerli.LinqToRest
{
    internal static class Pluralizer
    {
        public static string Pluralize(string noun)
            // Todo: search NuGet for Pluralisation
            => noun switch
            {
                _ when noun.EndsWithConsonantAndY() => noun[..^1] + "ies",
                _ => noun + "s",
            };

        private static bool EndsWithConsonantAndY(this string noun)
            => noun.EndsWith('y') && noun[^1].IsConsonant();

        private static bool IsConsonant(this char letter)
            => !letter.IsVowel();

        private static bool IsVowel(this char letter)
            => "aeiou".Contains(letter);
    }
}
