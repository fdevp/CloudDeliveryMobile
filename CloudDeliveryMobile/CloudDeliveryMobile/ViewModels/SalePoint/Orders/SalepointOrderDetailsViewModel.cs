using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint.Orders
{
    public class SalepointOrderDetailsViewModel : BaseViewModel<int>
    {
        public OrderDetails Order { get; set; }

        //added orders interaction
        private MvxInteraction _orderReadyInteraction = new MvxInteraction();

        public IMvxInteraction OrderReadyInteraction => _orderReadyInteraction;

        private void SendAddedOrdersInteraction(object sender, EventArgs e)
        {
            this._orderReadyInteraction?.Raise();
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

        public SalepointOrderDetailsViewModel(IMvxNavigationService navigationService, ISalepointOrdersService ordersService)
        {
            this.navigationService = navigationService;
            this.ordersService = ordersService;
        }


        public async override void Start()
        {
            base.Start();
            this.InProgress = true;
            await this.ordersService.Details(this.orderId).ContinueWith(t =>
            {
                if (t.Exception != null)
                {

                    this.InProgress = false;
                    return;
                }

                this.Order = t.Result;
                RaisePropertyChanged(() => this.Order);
                this.InProgress = false;
                SendAddedOrdersInteraction(this, null);
            });

        }

        public override void Prepare(int orderId)
        {
            this.orderId = orderId;
        }

        int orderId;
        IMvxNavigationService navigationService;
        ISalepointOrdersService ordersService;
    }
}
