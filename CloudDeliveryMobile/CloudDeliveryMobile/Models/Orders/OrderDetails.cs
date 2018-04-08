using CloudDeliveryMobile.Models.Enums;
using Newtonsoft.Json;
using System;

namespace CloudDeliveryMobile.Models.Orders
{
    public class OrderDetails : Order
    {
        public string SalepointName { get; set; }

        public int SalepointId { get; set; }

        public string CarrierName { get; set; }

        public int? CarrierId { get; set; }

        [JsonProperty(PropertyName = "SalepointLatLng")]
        public string SalepointLatLngString { get; set; }

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

        public DateTime? AcceptedTime { get; set; }

        public DateTime? CancellationTime { get; set; }

        public DateTime? PickUpTime { get; set; }

        public DateTime? DeliveredTime { get; set; }

        public int? FinalMinutes { get; set; }

        private GeoPosition salepointLatLng;
    }
}
