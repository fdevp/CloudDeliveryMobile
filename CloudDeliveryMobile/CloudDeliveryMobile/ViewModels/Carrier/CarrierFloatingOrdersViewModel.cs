using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<OrderListItem> Orders
        {
            get
            {
                return this.ordersService.PendingOrders;
            }
        }


        public CarrierFloatingOrdersViewModel(IMvxNavigationService navigationService, IOrdersService ordersService)
        {
            this.navigationService = navigationService;
            this.ordersService = ordersService;
        }

        public override void Start()
        {
            
        }

        private IMvxNavigationService navigationService;
        private IOrdersService ordersService;
    }
}
