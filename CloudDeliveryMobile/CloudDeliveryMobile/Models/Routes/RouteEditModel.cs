using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Routes
{
    public class RouteEditModel
    {
        public List<RoutePoint> EditPoints { get; set; } = new List<RoutePoint>();

        public GeoPosition StartPosition { get; set; }
    }
}
