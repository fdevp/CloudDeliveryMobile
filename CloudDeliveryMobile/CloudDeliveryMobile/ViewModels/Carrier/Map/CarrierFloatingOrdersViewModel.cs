using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System.Collections.Generic;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierFloatingOrdersViewModel : BaseViewModel
    {
        public MvxCommand CloseFragment
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.navigationService.Close(this);
                });
            }
        }

        public List<OrderCarrier> Orders
        {
            get
            {
                return this.ordersService.PendingOrders;
            }
        }


        public CarrierFloatingOrdersViewModel(IMvxNavigationService navigationService, ICarrierOrdersService ordersService)
        {
            this.navigationService = navigationService;
            this.ordersService = ordersService;
        }

        public override void Start()
        {
            
        }

        private IMvxNavigationService navigationService;
        private ICarrierOrdersService ordersService;
    }
}
