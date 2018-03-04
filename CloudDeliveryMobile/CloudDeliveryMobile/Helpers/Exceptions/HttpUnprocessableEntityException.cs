using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Helpers.Exceptions
{
    public class HttpUnprocessableEntityException : Exception
    {
        public HttpUnprocessableEntityException(string message) : base(message) { }
    }
}
