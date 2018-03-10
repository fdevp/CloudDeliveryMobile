using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System.Linq;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierFloatingOrderDetailsViewModel : BaseViewModel<int>
    {
        public Order Order { get; set; }

        public string EndPointText
        {
            get
            {
                return string.Concat("Punkt dostawy \n", this.Order.DestinationCity, ", ", this.Order.DestinationAddress);
            }
        }

        public string SalepointText
        {
            get
            {
                return string.Concat(this.Order.SalepointName, "\n", this.Order.SalepointCity, ", ", this.Order.SalepointAddress);
            }
        }


        public MvxAsyncCommand AcceptOrder
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    this.InProgress = true;
                    await this.ordersService.Accept(this.Order);
                    this.InProgress = false;
                });
            }
        }


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

        public CarrierFloatingOrderDetailsViewModel(IMvxNavigationService navigationService, IOrdersService ordersService)
        {
            this.ordersService = ordersService;
            this.navigationService = navigationService;
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

        private IMvxNavigationService navigationService;
        private IOrdersService ordersService;
    }
}
