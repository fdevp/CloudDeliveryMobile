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

        private void SendOrderReadyInteraction(object sender, EventArgs e)
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

        public IMvxAsyncCommand ReloadData
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    this.ErrorOccured = false;
                    await this.InitializeOrder();
                });
            }
        }

        private async Task InitializeOrder()
        {
            if (!this.orderId.HasValue)
                return;

            this.InProgress = true;
            try
            {
                this.Order = await this.ordersService.Details(this.orderId.Value);
                RaisePropertyChanged(() => this.Order);
                SendOrderReadyInteraction(this, null);
            }
            catch (Exception e)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = e.Message;
            }

            this.InProgress = false;
        }

        public SalepointOrderDetailsViewModel(IMvxNavigationService navigationService, ISalepointOrdersService ordersService)
        {
            this.navigationService = navigationService;
            this.ordersService = ordersService;
        }


        public override async void Start()
        {
            base.Start();
            await this.InitializeOrder();
        }

        public override void Prepare(int orderId)
        {
            this.orderId = orderId;
        }

        int? orderId;
        IMvxNavigationService navigationService;
        ISalepointOrdersService ordersService;
    }
}
