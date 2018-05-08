using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using Microsoft.AspNet.SignalR.Client;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;

using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierMapViewModel : BaseViewModel
    {
        public bool ActiveRouteMode
        {
            get
            {
                return this.activeRouteMode;
            }
            set
            {
                this.activeRouteMode = value;
                RaisePropertyChanged(() => this.ActiveRouteMode);
            }
        }

        //collections
        public bool DataInitialised { get; set; } = false;

        public IMvxAsyncCommand InitializeSideView
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    if (!DataInitialised)
                    {
                        this.InProgress = true;
                        try
                        {
                            await this.routesService.ActiveRouteDetails();
                            this.ActiveRouteMode = true;
                        }
                        catch (HttpRequestException) //no connection
                        {
                            this.sideView.ErrorOccured = true;
                            this.sideView.ErrorMessage = "Problem z połączeniem z serwerem.";
                        }
                        catch (ApiException e)
                        {
                            await this.ordersService.GetPendingOrders();
                        }

                        await this.navigationService.Navigate(this.sideView);

                        this.InProgress = false;
                        DataInitialised = true;
                    }
                });
            }
        }

        public List<OrderCarrier> PendingOrders
        {
            get
            {
                return this.ordersService.PendingOrders;
            }
        }

        public RouteDetails ActiveRoute
        {
            get
            {
                return this.routesService.ActiveRoute;
            }
        }

        //interactions
        private MvxInteraction<ServiceEvent<CarrierOrdersEvents>> _ordersUpdateInteraction = new MvxInteraction<ServiceEvent<CarrierOrdersEvents>>();
        public IMvxInteraction<ServiceEvent<CarrierOrdersEvents>> OrdersUpdateInteraction => _ordersUpdateInteraction;


        private MvxInteraction<ServiceEvent<CarrierRouteEvents>> _routeUpdateInteraction = new MvxInteraction<ServiceEvent<CarrierRouteEvents>>();
        public IMvxInteraction<ServiceEvent<CarrierRouteEvents>> RouteUpdateInteraction => _routeUpdateInteraction;


        //SelectedSalepointId
        private MvxInteraction<int?> _selectedSalepointUpdateInteraction = new MvxInteraction<int?>();
        public IMvxInteraction<int?> SelectedSalepointUpdate => _selectedSalepointUpdateInteraction;


        //signalr
        public ConnectionState SignalrConnectionStatus
        {
            get
            {
                return this.notificationsProvider.SocketStatus;
            }
        }

        public IMvxAsyncCommand SignalrReconnect
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    if (this.notificationsProvider.SocketStatus == ConnectionState.Disconnected)
                        await this.notificationsProvider.StarListening();
                });
            }
        }

        //floating elements
        public int? SelectedSalepointId
        {
            get
            {
                return this.selectedSalepointId;
            }
            set
            {
                if (this.selectedSalepointId != value)
                {
                    this.selectedSalepointId = value;
                    this._selectedSalepointUpdateInteraction.Raise(value);
                }
            }
        }

        public IMvxCommand ShowFloatingOrders
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.navigationService.Navigate(this.floatingOrdersViewModel);
                });
            }
        }

        public IMvxCommand<int> ShowOrderDetails
        {
            get
            {
                return new MvxCommand<int>(id =>
                {
                    this.navigationService.Navigate<CarrierFloatingOrderDetailsViewModel, int>(id);
                });
            }
        }

        public GeoPosition BasePosition
        {
            get
            {
                if (this.basePosition != null)
                    return this.basePosition;

                this.basePosition = new GeoPosition { lat = Double.Parse(Resources.ConstantValues.map_base_lat, CultureInfo.InvariantCulture), lng = Double.Parse(Resources.ConstantValues.map_base_lng, CultureInfo.InvariantCulture) };

                return this.basePosition;
            }
        }

        public float BaseZoom
        {
            get
            {
                if (this.baseZoom.HasValue)
                    return this.baseZoom.Value;

                this.baseZoom = float.Parse(Resources.ConstantValues.map_base_zoom, CultureInfo.InvariantCulture);
                return this.baseZoom.Value;
            }
        }

        public CarrierMapViewModel(IDeviceProvider deviceProvider, IMvxNavigationService navigationService, ICarrierOrdersService ordersService, IRoutesService routesService, INotificationsProvider notificationsProvider)
        {
            this.deviceProvider = deviceProvider;
            this.navigationService = navigationService;
            this.ordersService = ordersService;
            this.routesService = routesService;
            this.notificationsProvider = notificationsProvider;

            this.notificationsProvider.SocketStatusUpdated += (sender, value) => RaisePropertyChanged(() => this.SignalrConnectionStatus);


            this.ordersService.PendingOrdersUpdated += this.PendingOrdersEventHandler;
            this.routesService.ActiveRouteUpdated += this.ActiveRouteEventHandler;

            this.sideView = Mvx.IocConstruct<CarrierSideViewViewModel>();
            this.floatingOrdersViewModel = Mvx.IocConstruct<CarrierFloatingOrdersViewModel>();
        }


        private void ActiveRouteEventHandler(object sender, ServiceEvent<CarrierRouteEvents> e)
        {
            switch (e.Type)
            {
                case CarrierRouteEvents.FinishedRoute:
                    this.ActiveRouteMode = false;
                    this.ordersService.GetPendingOrders();
                    break;
                case CarrierRouteEvents.AddedRoute:
                    this.ActiveRouteMode = true;
                    break;
            }

            _routeUpdateInteraction.Raise(e);
        }

        private void PendingOrdersEventHandler(object sender, ServiceEvent<CarrierOrdersEvents> e)
        {
            _ordersUpdateInteraction.Raise(e);
            RaisePropertyChanged(() => this.PendingOrders);
        }


        private bool activeRouteMode = false;

        private int? selectedSalepointId;
        private float? baseZoom;
        private GeoPosition basePosition;
        private GeoPosition currentPosition = new GeoPosition();

        private CarrierFloatingOrdersViewModel floatingOrdersViewModel;
        private CarrierSideViewViewModel sideView;


        private IDeviceProvider deviceProvider;
        private IMvxNavigationService navigationService;
        private INotificationsProvider notificationsProvider;
        private ICarrierOrdersService ordersService;
        private IRoutesService routesService;
    }
}
