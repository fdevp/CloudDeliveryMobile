using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Routes
{
    public class RoutePoint
    {
        public int Index { get; set; }

        public RoutePointType Type { get; set; }

        public int OrderId { get; set; }

        public DateTime? PassedTime { get; set; }

        public Order Order { get; set; }

        public string LatLng { get; set; }
    }
}
