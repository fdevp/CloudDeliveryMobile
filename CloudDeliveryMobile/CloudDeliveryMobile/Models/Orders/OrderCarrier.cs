using CloudDeliveryMobile.Models.Enums;
using Newtonsoft.Json;
using System;

namespace CloudDeliveryMobile.Models.Orders
{
    public class OrderCarrier : Order
    {
        public string SalepointName { get; set; }

        public string SalepointCity { get; set; }

        public string SalepointAddress { get; set; }

        public int SalepointId { get; set; }

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

        private GeoPosition salepointLatLng;
        
    }
}
