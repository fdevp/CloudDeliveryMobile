using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint.Map
{
    public class SalepointFloatingOrderDetailsViewModel : BaseViewModel<int>
    {
        public OrderSalepoint Order { get; set; }


        public MvxAsyncCommand CloseFragment
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    await this.navigationService.Close(this);
                });
            }
        }
        public override void Prepare(int orderId)
        {
            var order = this.ordersService.AddedOrders.Where(x => x.Id == orderId).FirstOrDefault();

            if (order == null)
            {
                order = this.ordersService.InProgressOrders.Where(x => x.Id == orderId).FirstOrDefault();
                if (order == null)
                {

                }
            }

            this.Order = order;
        }

        public SalepointFloatingOrderDetailsViewModel(IMvxNavigationService navigationService, ISalepointOrdersService ordersService)
        {
            this.ordersService = ordersService;
            this.navigationService = navigationService;
        }

        private IMvxNavigationService navigationService;
        private ISalepointOrdersService ordersService;
    }
}
