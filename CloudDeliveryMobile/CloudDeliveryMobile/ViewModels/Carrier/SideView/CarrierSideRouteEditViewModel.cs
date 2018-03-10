using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using CloudDeliveryMobile.Models.Routes.Edit;
using CloudDeliveryMobile.Models;

namespace CloudDeliveryMobile.ViewModels.Carrier.SideView
{
    public class CarrierSideRouteEditViewModel : BaseViewModel
    {
        public MvxObservableCollection<RoutePointEditModel> Points { get; set; } = new MvxObservableCollection<RoutePointEditModel>();

        public int SalepointsCount => Points.Where(x => x.Type == RoutePointType.SalePoint).Count();

        public int EndpointsCount => Points.Where(x => x.Type == RoutePointType.EndPoint).Count();

        public IMvxCommand<RouteMoveEditPoint> MovePoint
        {
            get
            {
                return new MvxCommand<RouteMoveEditPoint>(model =>
                {
                    this.Points.Move(model.SourceIndex, model.DestinationIndex);
                });
            }
        }

        public IMvxCommand<RoutePointEditModel> RemoveSalePointRoutePoint
        {
            get
            {
                return new MvxCommand<RoutePointEditModel>(routePoint =>
                {
                    Points.Remove(routePoint);
                    RaiseAllPropertiesChanged();
                });
            }
        }

        public IMvxCommand<RoutePointEditModel> AddSalePointRoutePoint
        {
            get
            {
                return new MvxCommand<RoutePointEditModel>(routePoint =>
                {
                    RoutePointEditModel salepoint = new RoutePointEditModel(this);
                    salepoint.Type = RoutePointType.SalePoint;
                    salepoint.OrderId = routePoint.OrderId;
                    salepoint.Order = routePoint.Order;

                    this.Points.Insert(this.Points.IndexOf(routePoint), salepoint);

                    RaiseAllPropertiesChanged();
                });
            }
        }

        public IMvxAsyncCommand AcceptRoute
        {
            get
            {

                return new MvxAsyncCommand(async () =>
               {
                   this.InProgress = true;
                   await this.routesService.Add(this.Points.ToList());
                   this.InProgress = false;
               });
            }
        }

        public CarrierSideRouteEditViewModel(IRoutesService routesService, IOrdersService ordersService, IMvxNavigationService navigationService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;
            this.ordersService = ordersService;

            this.ordersService.AcceptedOrdersUpdated += updatePoints;
        }

        public override async void Start()
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
            catch (HttpUnprocessableEntityException e) // server error
            {

            }
            catch (HttpRequestException e) //no connection
            {
                this.Error.Occured = true;
                this.Error.Message = "Problem z połączeniem z serwerem.";
            }
        }


        private void updatePoints(object sender, EventArgs e)
        {

            if (this.ordersService.AcceptedOrders == null)          //points have been cleared
            {
                this.Points = new MvxObservableCollection<RoutePointEditModel>();
                RaiseAllPropertiesChanged();
                return;
            }

            List<Order> addedOrders = this.ordersService.AcceptedOrders.Where(x => this.Points.All(y => y.OrderId != x.Id)).ToList(); //added points
            foreach (Order order in addedOrders)
            {
                this.setPoints(order);
            }

            RaiseAllPropertiesChanged();
        }

        private void setPoints(Order order)
        {

            RoutePointEditModel salepoint = new RoutePointEditModel(this);
            salepoint.Type = RoutePointType.SalePoint;
            salepoint.OrderId = order.Id;
            salepoint.Order = order;

            this.Points.Add(salepoint);

            RoutePointEditModel endpoint = new RoutePointEditModel(this);
            endpoint.Type = RoutePointType.EndPoint;
            endpoint.OrderId = order.Id;
            endpoint.Order = order;

            this.Points.Add(endpoint);
        }




        private bool initialised = false;
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private IOrdersService ordersService;
    }
}
