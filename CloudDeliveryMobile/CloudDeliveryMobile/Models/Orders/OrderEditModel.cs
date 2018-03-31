using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Orders
{
    public class OrderEditModel
    {
        public DateTime? RequiredPickUpTime { get; set; }

        public string DestinationCity { get; set; }

        public string DestinationAddress { get; set; }

        public GeoPosition EndLatLng { get; set; }

        public int Priority { get; set; }

        public int PackageId { get; set; }

        public string CustomerPhone { get; set; }

        public decimal? Price { get; set; }
    }
}
