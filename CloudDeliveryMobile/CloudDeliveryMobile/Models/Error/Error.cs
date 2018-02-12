using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Error
{
    public class Error
    {
        public bool Occured { get; set; } = false;

        public ErrorType Type { get; set; }

        public string Message { get; set; }
    }
}
