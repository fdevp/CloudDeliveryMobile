using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Enums
{
    public enum OrderStatus : int
    {
        Cancelled = 0,
        Added,
        Accepted,
        InDelivery,
        Delivered
    }
}
