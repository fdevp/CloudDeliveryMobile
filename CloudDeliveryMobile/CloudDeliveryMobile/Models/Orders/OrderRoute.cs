using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Orders
{
    public class OrderRoute : OrderCarrier
    {
        public DateTime? AcceptedTime { get; set; }
 
         public DateTime? PickUpTime { get; set; }
 
         public DateTime? DeliveredTime { get; set; }
 
         public string CustomerPhone { get; set; }
 
         public string SalepointPhone { get; set; }
 
         public decimal? Price { get; set; }
    }
}
