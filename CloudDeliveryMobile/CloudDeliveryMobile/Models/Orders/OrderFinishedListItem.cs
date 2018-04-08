using CloudDeliveryMobile.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Orders
{
    public class OrderFinishedListItem
    {
        public int Id { get; set; }

        public string Address
        {
            get
            {
                return string.Concat(DestinationCity, ", ", DestinationAddress);
            }
        }

        public string CarrierName { get; set; }

        public DateTime? AddedTime { get; set; }

        public string DestinationCity { get; set; }

        public string DestinationAddress { get; set; }

        public DateTime? DeliveredTime { get; set; }

        public DateTime? CancellationTime { get; set; }

        public OrderStatus Status { get; set; }
    }
}
