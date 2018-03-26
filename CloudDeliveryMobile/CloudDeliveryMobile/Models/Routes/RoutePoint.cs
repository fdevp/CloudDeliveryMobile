using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using System;

namespace CloudDeliveryMobile.Models.Routes
{
    public class RoutePoint
    {
        public int Id { get; set; }

        public int Index { get; set; }

        public RoutePointType Type { get; set; }

        public int OrderId { get; set; }

        public DateTime? PassedTime { get; set; }

        public OrderRouteDetails Order { get; set; }
    }
}
