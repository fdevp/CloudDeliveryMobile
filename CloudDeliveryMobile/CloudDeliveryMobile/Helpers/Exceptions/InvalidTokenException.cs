using System;

namespace CloudDeliveryMobile.Helpers.Exceptions
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message) : base(message) { }
    }
}
