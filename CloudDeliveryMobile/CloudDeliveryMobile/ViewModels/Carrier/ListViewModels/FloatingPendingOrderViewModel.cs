using Acr.UserDialogs;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier.ListViewModels
{
    public class FloatingPendingOrderViewModel : BaseViewModel
    {
        public OrderCarrier Order { get; set; }

        public IMvxAsyncCommand AcceptOrder
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
                    catch (Exception e)
                    {
                        this.dialogsService.Toast(e.Message, TimeSpan.FromSeconds(5));
                    }
                    

                    this.InProgress = false;

                });
            }
        }

        public FloatingPendingOrderViewModel(ICarrierOrdersService ordersService,IUserDialogs dialogsService)
        {
            this.dialogsService = dialogsService;
            this.ordersService = ordersService;
        }

        private IUserDialogs dialogsService;
        private ICarrierOrdersService ordersService;
    }
}
