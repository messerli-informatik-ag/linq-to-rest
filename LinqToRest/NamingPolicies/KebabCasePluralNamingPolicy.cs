using Messerli.ChangeCase;

namespace Messerli.LinqToRest.NamingPolicies
{
    internal sealed class KebabCasePluralNamingPolicy : INamingPolicy
    {
        public string ConvertName(string name) => Pluralizer.Pluralize(name).ToKebabCase();
    }
}
