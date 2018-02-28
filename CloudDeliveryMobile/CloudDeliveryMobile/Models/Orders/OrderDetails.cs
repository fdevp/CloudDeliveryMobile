using CloudDeliveryMobile.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Orders
{
    public class OrderDetails
    {
        public int Id { get; set; }

        public string SalepointName { get; set; }

        public int SalepointId { get; set; }

        public string CarrierName { get; set; }

        public int? CarrierId { get; set; }

        public string SalepointLatLng { get; set; }

        public DateTime? AddedTime { get; set; }

        public DateTime? AcceptedTime { get; set; }

        public DateTime? CancellationTime { get; set; }

        public DateTime? RequiredPickUpTime { get; set; }

        public DateTime? PickUpTime { get; set; }

        public DateTime? DeliveredTime { get; set; }

        public string StartLatLng { get; set; }

        public string DestinationCity { get; set; }

        public string DestinationAddress { get; set; }

        public int Priority { get; set; }

        public OrderStatus Status { get; set; }

        public string TraceJSON { get; set; }

        public string EndLatLng { get; set; }

        public int? DistanceMeters { get; set; }

        public int? ExpectedMinutes { get; set; }

        public int? FinalMinutes { get; set; }
    }
}
