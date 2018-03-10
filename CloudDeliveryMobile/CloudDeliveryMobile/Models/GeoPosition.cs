using System.Collections.Generic;

namespace CloudDeliveryMobile.Models
{
    public class GeoPosition
    {
        public double lat { get; set; }
        public double lng { get; set; }

        public Dictionary<string,string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            dict.Add("lat", this.lat.ToString());
            dict.Add("lng", this.lng.ToString());
            return dict;
        }
    }
}
