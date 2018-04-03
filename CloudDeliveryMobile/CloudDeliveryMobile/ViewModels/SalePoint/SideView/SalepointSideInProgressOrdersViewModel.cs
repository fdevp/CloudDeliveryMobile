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

        public SalepointSideInProgressOrdersViewModel(ISalepointOrdersService salepointOrdersService)
        {
            this.salepointOrdersService = salepointOrdersService;
            this.salepointOrdersService.InProgressOrdersUpdated += OrdersPropertyChanged;

            this.Orders = new MvxObservableCollection<SalepointOrderListItemViewModel>();
        }

        private void OrdersPropertyChanged(object sender, EventArgs e)
        {
            CreateOrdersViewModels();
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
                await this.salepointOrdersService.GetInProgressOrders();
            }
            catch (Exception e)
            {
                this.Error.Occured = true;
                this.Error.Message = "Problem z połączeniem z serwerem.";
            }
            finally
            {
                this.InProgress = false;
            }
        }

        private void CreateOrdersViewModels()
        {
            List<OrderSalepoint> updatedOrders = this.salepointOrdersService.InProgressOrders;

            List<OrderSalepoint> removedOrders = this.Orders.Where(x => updatedOrders.All(y => y.Id != x.Order.Id)).Select(x => x.Order).ToList();
            foreach (var item in removedOrders)
            {
                var toRemove = this.Orders.Where(x => x.Order == item).FirstOrDefault();
                if (toRemove != null)
                    this.Orders.Remove(toRemove);
            }

            List<OrderSalepoint> newOrders = this.salepointOrdersService.InProgressOrders.Where(x => this.Orders.All(y => y.Order.Id != x.Id))
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
    }
}
