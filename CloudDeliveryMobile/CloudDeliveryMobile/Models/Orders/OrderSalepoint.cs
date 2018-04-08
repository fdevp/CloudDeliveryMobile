using CloudDeliveryMobile.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Orders
{
    public class OrderSalepoint : Order
    {
        public string CarrierName { get; set; }

        public string CarrierPhone { get; set; }

        public int? CarrierId { get; set; }

        public DateTime? AcceptedTime { get; set; }

        public DateTime? PickUpTime { get; set; }

    }
}
