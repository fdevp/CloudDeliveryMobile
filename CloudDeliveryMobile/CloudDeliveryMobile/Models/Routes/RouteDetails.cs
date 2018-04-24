using CloudDeliveryMobile.Models.Enums;
using System;
using System.Collections.Generic;

namespace CloudDeliveryMobile.Models.Routes
{
    public class RouteDetails
    {
        public int Id { get; set; }

        public int CarrierId { get; set; }

        public string CarrierName { get; set; }

        public int RoutePointsCount { get; set; }

        public RouteStatus Status { get; set; }

        public DateTime AddedTime { get; set; }

        public DateTime? FinishTime { get; set; }

        public int? Duration { get; set; }

        public List<RoutePoint> Points { get; set; }
    }
}
