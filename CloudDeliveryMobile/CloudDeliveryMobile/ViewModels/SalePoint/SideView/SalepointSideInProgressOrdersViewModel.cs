using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint.SideView
{
    public class SalepointSideInProgressOrdersViewModel : BaseViewModel
    {
        public MvxObservableCollection<SalepointOrderListItemViewModel> Orders { get; set; }

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

        public SalepointSideInProgressOrdersViewModel(ISalepointOrdersService salepointOrdersService)
        {
            this.salepointOrdersService = salepointOrdersService;
            this.salepointOrdersService.InProgressOrdersUpdated += OrdersPropertyChanged;

            this.Orders = new MvxObservableCollection<SalepointOrderListItemViewModel>();
        }
  
        public async override void Start()
        {
            if (initialised)
                return;
            initialised = true;

            base.Start();
            await this.InitializeData();

        }

        private async Task InitializeData()
        {
            this.InProgress = true;
            try
            {
                await this.salepointOrdersService.GetInProgressOrders();
            }
            catch (Exception e)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = "Problem z połączeniem z serwerem.";
            }

            this.InProgress = false;
        }

        private void OrdersPropertyChanged(object sender, ServiceEvent<SalepointInProgressOrdersEvents> e)
        {
            switch (e.Type)
            {
                case SalepointInProgressOrdersEvents.AddedList:
                    this.Orders.Clear();
                    foreach (var item in this.salepointOrdersService.InProgressOrders)
                        CreateOrderViewModel(item);
                    break;
                case SalepointInProgressOrdersEvents.AddedOrder:
                    this.CreateOrderViewModel((OrderSalepoint)e.Resource);
                    break;
                case SalepointInProgressOrdersEvents.RemovedOrder:
                    var orderToRemove = (OrderSalepoint)e.Resource;
                    var orderVM = this.Orders.Where(x => x.Order.Id == orderToRemove.Id).FirstOrDefault();
                    if (orderVM != null)
                        this.Orders.Remove(orderVM);
                    break;
                case SalepointInProgressOrdersEvents.PickedOrder:
                    var orderToChange = (OrderSalepoint)e.Resource;
                    var orderPickedVM = this.Orders.Where(x => x.Order.Id == orderToChange.Id).FirstOrDefault();
                    if (orderPickedVM != null)
                        orderPickedVM.RaiseAllPropertiesChanged();
                    break;
            }
            RaisePropertyChanged(() => this.Orders);
        }

        private void CreateOrderViewModel(OrderSalepoint order)
        {
            var orderVM = Mvx.IocConstruct<SalepointOrderListItemViewModel>();
            orderVM.Order = order;
            this.Orders.Add(orderVM);
        }

        bool initialised = false;
        ISalepointOrdersService salepointOrdersService;
    }
}
