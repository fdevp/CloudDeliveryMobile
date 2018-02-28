using CloudDeliveryMobile.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Orders
{
    public class OrderListItem
    {
        public int Id { get; set; }

        public string SalepointName { get; set; }

        public string SalepointCity { get; set; }

        public string SalepointAddress { get; set; }

        public int SalepointId { get; set; }

        [JsonProperty(PropertyName = "SalepointLatLng")]
        private string SalepointLatLngString { get; set; }

        [JsonIgnore]
        public GeoPosition SalepointLatLng
        {
            get
            {
                if (this.salepointLatLng != null)
                    return this.salepointLatLng;
                this.salepointLatLng = JsonConvert.DeserializeObject<GeoPosition>(this.SalepointLatLngString);
                return this.salepointLatLng;
            }
        }

        public DateTime AddedTime { get; set; }

        public DateTime? RequiredPickUpTime { get; set; }

        public string DestinationCity { get; set; }

        public string DestinationAddress { get; set; }

        public int Priority { get; set; }

        public OrderStatus Status { get; set; }


        [JsonProperty(PropertyName = "EndLatLng")]
        private string EndLatLngString { get; set; }

        [JsonIgnore]
        public GeoPosition EndLatLng
        {
            get
            {
                if (this.endLatLng != null)
                    return this.endLatLng;
                this.endLatLng = JsonConvert.DeserializeObject<GeoPosition>(this.EndLatLngString);
                return this.endLatLng;
            }
        }

        private GeoPosition salepointLatLng;
        private GeoPosition endLatLng;
    }
}
