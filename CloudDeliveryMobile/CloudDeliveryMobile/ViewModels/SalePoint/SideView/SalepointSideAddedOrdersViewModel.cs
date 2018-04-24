using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint.SideView
{
    public class SalepointSideAddedOrdersViewModel : BaseViewModel
    {
        public MvxObservableCollection<SalepointOrderListItemViewModel> Orders { get; set; }

        public IMvxAsyncCommand OpenNewOrderModal
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    await this.navigationService.Navigate(typeof(SalepointNewOrderViewModel));
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
                    await this.InitializeData();
                });
            }
        }

        public SalepointSideAddedOrdersViewModel(IMvxNavigationService navigationService, ISalepointOrdersService salepointOrdersService)
        {
            this.navigationService = navigationService;
            this.salepointOrdersService = salepointOrdersService;
            this.salepointOrdersService.AddedOrdersUpdated += OrdersPropertyChanged;

            this.Orders = new MvxObservableCollection<SalepointOrderListItemViewModel>();
        }

        public async override void Start()
        {
            if (initialised)
                return;
            initialised = true;

            base.Start();

            await InitializeData();

        }

        private async Task InitializeData()
        {
            this.InProgress = true;

            try
            {
                await this.salepointOrdersService.GetAddedOrders();
            }
            catch (Exception e)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = "Problem z połączeniem z serwerem.";
            }

            this.InProgress = false;

        }

        private void OrdersPropertyChanged(object sender, ServiceEvent<SalepointAddedOrdersEvents> e)
        {
            switch (e.Type)
            {
                case SalepointAddedOrdersEvents.AddedList:
                    this.Orders.Clear();
                    foreach (var item in this.salepointOrdersService.AddedOrders)
                        CreateOrderViewModel(item);
                    break;
                case SalepointAddedOrdersEvents.AddedOrder:
                    this.CreateOrderViewModel((OrderSalepoint)e.Resource);
                    break;
                case SalepointAddedOrdersEvents.RemovedOrder:
                    var orderToRemove = (OrderSalepoint)e.Resource;
                    var orderVM = this.Orders.Where(x => x.Order.Id == orderToRemove.Id).FirstOrDefault();
                    if (orderVM != null)
                        this.Orders.Remove(orderVM);
                    break;
            }
            this.RaisePropertyChanged(() => this.Orders);
        }

        private void CreateOrderViewModel(OrderSalepoint order)
        {
            var orderVM = Mvx.IocConstruct<SalepointOrderListItemViewModel>();
            orderVM.Order = order;
            this.Orders.Add(orderVM);
        }

        bool initialised = false;

        ISalepointOrdersService salepointOrdersService;
        IMvxNavigationService navigationService;
    }
}
