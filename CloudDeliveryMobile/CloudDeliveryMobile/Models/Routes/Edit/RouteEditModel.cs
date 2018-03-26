using CloudDeliveryMobile.Models.Enums;
using System.Collections.Generic;

namespace CloudDeliveryMobile.Models.Routes
{
    public class RouteEditModel
    {
        public int OrderId { get; set; }

        public int Index { get; set; }

        public RoutePointType Type { get; set; }
    }
}
