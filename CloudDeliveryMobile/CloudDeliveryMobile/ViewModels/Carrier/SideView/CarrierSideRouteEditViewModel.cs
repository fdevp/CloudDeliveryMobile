using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier.SideView
{
    public class CarrierSideRouteEditViewModel : BaseViewModel
    {
        public RouteEditModel Model { get; set; } = new RouteEditModel();

        public CarrierSideRouteEditViewModel(IRoutesService routesService, IOrdersService ordersService, IMvxNavigationService navigationService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;
            this.ordersService = ordersService;

            this.ordersService.AcceptedOrdersUpdated += updateEditModel;
        }

        public async override void Start()
        {
            base.Start();

            if (initialised)
                return;

            try
            {
                this.InProgress = true;
                await this.ordersService.GetAcceptedOrders();
                this.InProgress = false;
            }
            catch (HttpUnprocessableEntityException e) // no accepted orders
            {
                this.Model = new RouteEditModel();
            }
            catch (HttpRequestException e) //no connection
            {
                this.Error.Occured = true;
                this.Error.Message = "Problem z połączeniem z serwerem.";
            }
        }

        private void updateEditModel(object sender, EventArgs e)
        {

            if (this.ordersService.AcceptedOrders == null)          //points have been cleared
            {
                this.Model = new RouteEditModel();
                return;
            }

            List<Order> addedOrders = this.ordersService.AcceptedOrders.Where(x => !this.Model.EditPoints.Any(y => y.OrderId == x.Id)).ToList(); //added points
            int index = this.Model.EditPoints.Select(x => x.Index).DefaultIfEmpty(-1).Max() + 1; //new element index
            foreach (Order order in addedOrders)
            {
                this.setEditPoints(order, index);
                index += 2;
            }
        }

        private void setEditPoints(Order order, int index)
        {
            RoutePoint salepoint = new RoutePoint();
            salepoint.Type = RoutePointType.SalePoint;
            salepoint.Index = index;
            salepoint.OrderId = order.Id;
            salepoint.Order = order;

            this.Model.EditPoints.Add(salepoint);

            RoutePoint endpoint = new RoutePoint();
            endpoint.Type = RoutePointType.SalePoint;
            endpoint.Index = index + 1;
            endpoint.OrderId = order.Id;
            endpoint.Order = order;

            this.Model.EditPoints.Add(endpoint);
        }

        private bool initialised = false;
        private List<Order> acceptedOrders;
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private IOrdersService ordersService;
    }
}
