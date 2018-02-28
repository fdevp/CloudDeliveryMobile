using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierFloatingOrderDetailsViewModel : MvxViewModel<int>
    {
        public OrderListItem Order { get; set; }

        public string EndPointText
        {
            get
            {
                return string.Concat("Punkt dostawy \n",this.Order.DestinationCity, ", ", this.Order.DestinationAddress);
            }
        }

        public string SalepointText
        {
            get
            {
                return string.Concat(this.Order.SalepointName,"\n",this.Order.SalepointCity, ", ", this.Order.SalepointAddress);
            }
        }

        public CarrierFloatingOrderDetailsViewModel(IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        public override void Prepare(int orderId)
        {
            var order = this.ordersService.PendingOrders.Where(x => x.Id == orderId).FirstOrDefault();
            if (order == null)
            {
                //

            }

            this.Order = order;
        }

        private IOrdersService ordersService;
    }
}
