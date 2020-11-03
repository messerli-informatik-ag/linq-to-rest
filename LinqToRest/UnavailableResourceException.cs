using System;

namespace Messerli.LinqToRest
{
    public sealed class UnavailableResourceException : Exception
    {
        public UnavailableResourceException()
        {
        }

        public UnavailableResourceException(string message)
            : base(message)
        {
        }

        public UnavailableResourceException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
