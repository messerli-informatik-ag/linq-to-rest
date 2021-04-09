using System;

namespace Messerli.LinqToRest.NamingPolicies
{
    internal sealed class AnonymousNamingPolicy : INamingPolicy
    {
        private readonly Func<string, string> _convertName;

        public AnonymousNamingPolicy(Func<string, string> convertName) => _convertName = convertName;

        public string ConvertName(string name) => _convertName(name);
    }
}
