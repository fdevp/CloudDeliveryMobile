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
            finally
            {
                this.InProgress = false;
            }

        }

        private void OrdersPropertyChanged(object sender, EventArgs e)
        {
            CreateOrdersViewModels();
        }

        private void CreateOrdersViewModels()
        {
            List<OrderSalepoint> updatedOrders = this.salepointOrdersService.AddedOrders;

            List<OrderSalepoint> removedOrders = this.Orders.Where(x => updatedOrders.All(y => y.Id != x.Order.Id)).Select(x => x.Order).ToList();

            foreach(var item in removedOrders)
            {
                var toRemove = this.Orders.Where(x => x.Order == item).FirstOrDefault();
                if(toRemove!=null)
                    this.Orders.Remove(toRemove);
            }


            List<OrderSalepoint> newOrders = this.salepointOrdersService.AddedOrders.Where(x => this.Orders.All(y => y.Order.Id != x.Id))
                                                                                          .ToList();

            foreach (var item in newOrders)
            {
                var orderVM = Mvx.IocConstruct<SalepointOrderListItemViewModel>();
                orderVM.Order = item;
                this.Orders.Add(orderVM);
            }

            this.RaisePropertyChanged(() => this.Orders);
        }

        bool initialised = false;

        ISalepointOrdersService salepointOrdersService;
        IMvxNavigationService navigationService;
    }
}
