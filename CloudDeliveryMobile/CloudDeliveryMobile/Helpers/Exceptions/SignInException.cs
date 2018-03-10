using System;

namespace CloudDeliveryMobile.Helpers.Exceptions
{
    public class SignInException : Exception
    {
        public SignInException(string message) : base(message) { }
    }
}
