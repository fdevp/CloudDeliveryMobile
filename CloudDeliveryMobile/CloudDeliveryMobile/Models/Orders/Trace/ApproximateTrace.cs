using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.Models.Orders
{
    public class ApproximateTrace
    {
        public DurationDistance CarrierToSalePoint { get; set; } = new DurationDistance();

        public DurationDistance SalePointToEndpoint { get; set; } = new DurationDistance();
    }
}
