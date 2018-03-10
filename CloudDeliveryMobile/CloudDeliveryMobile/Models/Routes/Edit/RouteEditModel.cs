using System.Collections.Generic;
using CloudDeliveryMobile.Models.Routes.Edit;

namespace CloudDeliveryMobile.Models.Routes
{
    public class RouteEditModel
    {
        public List<RoutePoint> Points { get; set; } = new List<RoutePoint>();

        public GeoPosition StartPosition { get; set; }
    }
}
