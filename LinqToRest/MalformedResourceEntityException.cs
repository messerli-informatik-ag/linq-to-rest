using System;

namespace LinqToRest.LinqToRest
{
    public class MalformedResourceEntityException : Exception
    {
        public MalformedResourceEntityException()
        {
        }

        public MalformedResourceEntityException(string message)
            : base(message)
        {
        }

        public MalformedResourceEntityException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
