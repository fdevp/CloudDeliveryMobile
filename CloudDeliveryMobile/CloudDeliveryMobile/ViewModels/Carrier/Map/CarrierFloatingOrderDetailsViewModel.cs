using Acr.UserDialogs;
using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using Refit;
using System;
using System.Linq;
using System.Net.Http;

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
                    try
                    {
                        await this.ordersService.Accept(this.Order);
                    }
                    catch(ApiException ex)
                    {
                        this.dialogsService.Toast(ex.Message, TimeSpan.FromSeconds(5));
                    }
                    catch (HttpRequestException e)
                    {
                        this.dialogsService.Toast("Problem z połączeniem z serwerem.", TimeSpan.FromSeconds(5));
                    }

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

        public CarrierFloatingOrderDetailsViewModel(IMvxNavigationService navigationService, ICarrierOrdersService ordersService, IUserDialogs dialogsService)
        {
            this.ordersService = ordersService;
            this.navigationService = navigationService;
            this.dialogsService = dialogsService;
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
        private IUserDialogs dialogsService;
    }
}
