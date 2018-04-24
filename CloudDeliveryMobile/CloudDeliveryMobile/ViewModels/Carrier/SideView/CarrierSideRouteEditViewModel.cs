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
using Acr.UserDialogs;
using CloudDeliveryMobile.Models.Enums.Events;
using System.Threading.Tasks;

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

                   List<RouteEditModel> newRouteModel = CreateNewRouteModel();
                   try
                   {
                       await this.routesService.Add(newRouteModel);
                       this.ordersService.CleanAcceptedOrders();
                   }
                   catch (HttpUnprocessableEntityException e)
                   {
                       this.dialogsService.Toast(e.Message, TimeSpan.FromSeconds(5));
                   }
                   catch (HttpRequestException httpException)
                   {
                       this.dialogsService.Toast("Problem z połączeniem z serwerem.", TimeSpan.FromSeconds(5));
                   }

                   this.InProgress = false;
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

        public CarrierSideRouteEditViewModel(IRoutesService routesService, ICarrierOrdersService ordersService, IMvxNavigationService navigationService, IUserDialogs dialogsService)
        {
            this.routesService = routesService;
            this.navigationService = navigationService;
            this.ordersService = ordersService;
            this.dialogsService = dialogsService;

            this.ordersService.AcceptedOrdersUpdated += UpdatePoints;
        }

        public override async void Start()
        {
            base.Start();

            if (initialised)
                return;

            initialised = true;

            await InitializeData();
        }

        public async Task InitializeData()
        {

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
            }
            catch (Exception e)
            {
                this.ErrorOccured = true;
                this.ErrorMessage = "Wystąpił nieznany błąd.";
            }

            this.InProgress = false;

        }

        private void UpdatePoints(object sender, ServiceEvent<CarrierOrdersEvents> e)
        {

            switch (e.Type)
            {
                case CarrierOrdersEvents.AddedList:
                    foreach (OrderCarrier order in this.ordersService.AcceptedOrders)
                        this.SetPoints(order);
                    break;
                case CarrierOrdersEvents.RemovedList:
                    this.Points = new MvxObservableCollection<RoutePointEditListItem>();
                    RaiseAllPropertiesChanged();
                    break;
                case CarrierOrdersEvents.AddedOrder:
                    this.SetPoints((OrderCarrier)e.Resource);
                    break;
            }


            RaiseAllPropertiesChanged();
        }

        private void SetPoints(OrderCarrier order)
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

        private List<RouteEditModel> CreateNewRouteModel()
        {
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

            return newRoute;
        }

        private bool initialised = false;
        private IMvxNavigationService navigationService;
        private IRoutesService routesService;
        private ICarrierOrdersService ordersService;
        private IUserDialogs dialogsService;
    }
}
