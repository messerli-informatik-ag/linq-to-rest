using System;
using System.ComponentModel;
using System.Reflection;
using Funcky.Extensions;

namespace Messerli.LinqToRest
{
    public static class StringToEnumExtension
    {
        public static object TryParseToEnumElement(this string candidate, Type type)
        {
            if (!type.IsEnum)
            {
                throw new ArgumentException($"{type.Name} is not an enum type!");
            }

            var tryParse = typeof(StringToEnumExtension).GetMethod(nameof(TryParseToEnumElement), new Type[] {typeof(string) })?.MakeGenericMethod(type)
                           ?? throw new MissingMethodException();

            try
            {
                return tryParse.Invoke(null, new object[] { candidate });
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException != null)
                {
                    throw e.InnerException;
                }

                throw;
            }
        }

        public static T TryParseToEnumElement<T>(this string candidate) where T : struct
        {
            var parsed = candidate.TryParseEnum<T>();

            return parsed.Match(false, @enum => true)
                ? parsed.Match(default(T), @enum => @enum)
                : throw new InvalidEnumArgumentException(candidate);
        }
    }
}