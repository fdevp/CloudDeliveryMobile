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

        public static explicit operator OrderFinishedListItem(OrderSalepoint obj)
        {
            OrderFinishedListItem output = new OrderFinishedListItem
            {
                Id = obj.Id,
                Status = obj.Status,
                AddedTime = obj.AddedTime,
                CarrierName = obj.CarrierName,
                DestinationAddress = obj.DestinationAddress,
                DestinationCity = obj.DestinationCity
            };
            return output;
        }

    }
}
