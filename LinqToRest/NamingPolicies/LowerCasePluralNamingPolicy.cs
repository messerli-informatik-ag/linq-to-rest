namespace Messerli.LinqToRest.NamingPolicies
{
    internal sealed class LowerCasePluralNamingPolicy : INamingPolicy
    {
        public string ConvertName(string name) => Pluralizer.Pluralize(name.ToLower());
    }
}
