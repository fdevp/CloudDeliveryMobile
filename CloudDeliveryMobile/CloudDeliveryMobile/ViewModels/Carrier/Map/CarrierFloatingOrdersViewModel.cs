using Acr.UserDialogs;
using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.Carrier.ListViewModels;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierFloatingOrdersViewModel : BaseViewModel, IDisposable
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

        public MvxObservableCollection<FloatingPendingOrderViewModel> Orders { get; set; }

        public bool RefreshingDataInProgress { get; set; } = false;

        public IMvxAsyncCommand RefreshData
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    this.RefreshingDataInProgress = true;
                    RaisePropertyChanged(() => this.RefreshingDataInProgress);

                    try
                    {
                        await this.ordersService.GetPendingOrders();
                        dialogsService.Toast("Zaktualizowano zamówienia", TimeSpan.FromSeconds(5));
                    }
                    catch (ApiException ex)
                    {
                        dialogsService.Toast("Błąd aktualizacji zamówień", TimeSpan.FromSeconds(5));
                    }
                    catch (HttpRequestException)
                    {
                        dialogsService.Toast("Problem z połączniem z serwerem", TimeSpan.FromSeconds(5));
                    }


                    this.RefreshingDataInProgress = false;
                    RaisePropertyChanged(() => this.RefreshingDataInProgress);
                });
            }
        }

        public CarrierFloatingOrdersViewModel(IMvxNavigationService navigationService, ICarrierOrdersService ordersService, IUserDialogs dialogsService)
        {
            this.navigationService = navigationService;
            this.ordersService = ordersService;
            this.dialogsService = dialogsService;

            this.CreateOrderViewModelsList();
            this.ordersService.PendingOrdersUpdated += UpdateOrders;
        }

        private void UpdateOrders(object sender, ServiceEvent<CarrierOrdersEvents> e)
        {
            switch (e.Type)
            {
                case CarrierOrdersEvents.AddedList:
                    CreateOrderViewModelsList();
                    break;
                case CarrierOrdersEvents.AddedOrder:
                    AddOrderViewModel((OrderCarrier)e.Resource);
                    break;
                case CarrierOrdersEvents.RemovedList:
                    this.Orders.Clear();
                    break;
                case CarrierOrdersEvents.RemovedOrder:
                    OrderCarrier order = (OrderCarrier)e.Resource;
                    RemoveOrderViewModel(order);
                    break;

            }

            RaisePropertyChanged(() => this.Orders);
        }

        private void CreateOrderViewModelsList()
        {
            this.Orders = new MvxObservableCollection<FloatingPendingOrderViewModel>();

            if (this.ordersService.PendingOrders == null)
                return;

            foreach (OrderCarrier pendingOrder in this.ordersService.PendingOrders)
                AddOrderViewModel(pendingOrder);
        }

        private void AddOrderViewModel(OrderCarrier order)
        {
            var orderVM = Mvx.IocConstruct<FloatingPendingOrderViewModel>();
            orderVM.Order = order;
            this.Orders.Add(orderVM);
        }

        private void RemoveOrderViewModel(OrderCarrier order)
        {
            var orderToRemove = this.Orders.Where(x => x.Order.Id == order.Id).FirstOrDefault();
            if (orderToRemove != null)
                this.Orders.Remove(orderToRemove);
        }

        public void Dispose()
        {
            this.ordersService.PendingOrdersUpdated -= UpdateOrders;
        }

        private IMvxNavigationService navigationService;
        private ICarrierOrdersService ordersService;
        private IUserDialogs dialogsService;
    }
}
