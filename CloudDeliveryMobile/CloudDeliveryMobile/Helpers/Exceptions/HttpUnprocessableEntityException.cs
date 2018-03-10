using System;

namespace CloudDeliveryMobile.Helpers.Exceptions
{
    public class HttpUnprocessableEntityException : Exception
    {
        public HttpUnprocessableEntityException(string message) : base(message) { }
    }
}
