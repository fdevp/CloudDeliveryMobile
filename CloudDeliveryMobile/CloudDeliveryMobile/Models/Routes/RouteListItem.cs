using CloudDeliveryMobile.Models.Enums;
using System;

namespace CloudDeliveryMobile.Models.Routes
{
    public class RouteListItem
    {
        public int Id { get; set; }

        public int CarrierId { get; set; }

        public RouteStatus Status { get; set; }

        public DateTime AddedTime { get; set; }

        public DateTime FinishTime { get; set; }

        public string StartLatLng { get; set; }

        public int? Distance { get; set; }

        public int? Duration { get; set; }
    }
}
