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
using CloudDeliveryMobile.Models;

namespace CloudDeliveryMobile.ViewModels.Carrier.SideView
{
    public class CarrierSideRouteEditViewModel : BaseViewModel
    {
        public MvxObservableCollection<RoutePointEditListItem> Points { get; set; } = new MvxObservableCollection<RoutePointEditListItem>();

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

        public IMvxCommand<RoutePointEditListItem> RemoveSalePointRoutePoint
        {
            get
            {
                return new MvxCommand<RoutePointEditListItem>(routePoint =>
                {
                    Points.Remove(routePoint);
                    RaiseAllPropertiesChanged();
                });
            }
        }

        public IMvxCommand<RoutePointEditListItem> AddSalePointRoutePoint
        {
            get
            {
                return new MvxCommand<RoutePointEditListItem>(routePoint =>
                {
                    RoutePointEditListItem salepoint = new RoutePointEditListItem(this);
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
                   List<RouteEditModel> newRoute = new List<RouteEditModel>();

                   int index = 0;
                   foreach (RoutePointEditListItem point in Points)
                   {
                        newRoute.Add(new RouteEditModel
                        {
                            OrderId = point.OrderId,
                            Type = point.Type,
                            Index = index++
                       });
                   }

                   await this.routesService.Add(newRoute);
                   this.InProgress = false;
               });
            }
        }

        public CarrierSideRouteEditViewModel(IRoutesService routesService, ICarrierOrdersService ordersService, IMvxNavigationService navigationService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;
            this.ordersService = ordersService;

            this.ordersService.AcceptedOrdersUpdated += UpdatePoints;
        }
        
        public override async void Start()
        {
            base.Start();

            if (initialised)
                return;

            this.InProgress = true;
            try
            {
                await this.ordersService.GetAcceptedOrders();
            }
            catch (HttpUnprocessableEntityException e) // server error
            {
                this.ErrorOccured = true;
                this.ErrorMessage = e.Message;
            }
            catch (HttpRequestException httpException) //no connection
            {
                this.ErrorOccured = true;
                this.ErrorMessage = "Problem z połączeniem z serwerem.";
            }catch(Exception e)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = "Wystąpił nieznany błąd.";
            }
            finally
            {
                this.InProgress = false;
            }
        }

        private void UpdatePoints(object sender, EventArgs e)
        {

            if (this.ordersService.AcceptedOrders == null)          //points have been cleared
            {
                this.Points = new MvxObservableCollection<RoutePointEditListItem>();
                RaiseAllPropertiesChanged();
                return;
            }

            List<OrderCarrier> addedOrders = this.ordersService.AcceptedOrders.Where(x => this.Points.All(y => y.OrderId != x.Id)).ToList(); //added points
            foreach (OrderCarrier order in addedOrders)
            {
                this.setPoints(order);
            }

            RaiseAllPropertiesChanged();
        }

        private void setPoints(OrderCarrier order)
        {

            RoutePointEditListItem salepoint = new RoutePointEditListItem(this);
            salepoint.Type = RoutePointType.SalePoint;
            salepoint.OrderId = order.Id;
            salepoint.Order = order;

            this.Points.Add(salepoint);

            RoutePointEditListItem endpoint = new RoutePointEditListItem(this);
            endpoint.Type = RoutePointType.EndPoint;
            endpoint.OrderId = order.Id;
            endpoint.Order = order;

            this.Points.Add(endpoint);
        }

        private bool initialised = false;
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private ICarrierOrdersService ordersService;
    }
}
