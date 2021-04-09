using System;
using Messerli.LinqToRest.NamingPolicies;

namespace Messerli.LinqToRest
{
    public static class NamingPolicy
    {
        public static INamingPolicy KebabCasePlural { get; } = new KebabCasePluralNamingPolicy();

        public static INamingPolicy LowerCasePlural { get; } = new LowerCasePluralNamingPolicy();

        public static INamingPolicy Create(Func<string, string> convertName) => new AnonymousNamingPolicy(convertName);
    }
}
