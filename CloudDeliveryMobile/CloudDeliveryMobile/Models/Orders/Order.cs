using CloudDeliveryMobile.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Orders
{
    public abstract class Order
    {
        public int Id { get; set; }

        public DateTime AddedTime { get; set; }

        public DateTime? RequiredPickUpTime { get; set; }

        public string DestinationCity { get; set; }

        public string DestinationAddress { get; set; }

        public int Priority { get; set; }

        public OrderStatus Status { get; set; }

        [JsonProperty(PropertyName = "EndLatLng")]
        public string EndLatLngString { get; set; }

        [JsonIgnore]
        public GeoPosition EndLatLng
        {
            get
            {
                if (this.endLatLng != null)
                    return this.endLatLng;

                if (this.EndLatLngString == null)
                    return null;

                this.endLatLng = JsonConvert.DeserializeObject<GeoPosition>(this.EndLatLngString);
                return this.endLatLng;
            }
        }

        private GeoPosition endLatLng;

        public decimal? Price { get; set; }

        public string CustomerPhone { get; set; }
    }
}
