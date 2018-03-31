﻿using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System.Linq;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierFloatingOrderDetailsViewModel : BaseViewModel<int>
    {
        public OrderCarrier Order { get; set; }

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

        public CarrierFloatingOrderDetailsViewModel(IMvxNavigationService navigationService, ICarrierOrdersService ordersService)
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
        private ICarrierOrdersService ordersService;
    }
}
