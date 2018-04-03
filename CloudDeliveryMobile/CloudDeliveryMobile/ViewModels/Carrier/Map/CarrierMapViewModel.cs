﻿using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;

using System.Linq;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierMapViewModel : BaseViewModel
    {
        public CarrierSideViewViewModel SideView { get; set; }


        //collections
        private MvxInteraction _ordersUpdateInteraction = new MvxInteraction();

        public IMvxInteraction OrdersUpdateInteraction => _ordersUpdateInteraction;

        private void SendInteraction(object sender, EventArgs e)
        {
            this._ordersUpdateInteraction.Raise();
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

        public bool IsActiveRoute
        {
            get
            {
                return this.SideView.currentChildViewModel != null && this.SideView.currentChildViewModel.GetType() == typeof(CarrierSideActiveRouteViewModel);
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
                    this.SendInteraction(this, null);
                }
            }
        }

        public IMvxCommand ShowFloatingOrders
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this.navigationService.Navigate<CarrierFloatingOrdersViewModel>();
                });
            }
        }

        public IMvxCommand<int> ShowOrderDetails
        {
            get
            {
                return new MvxCommand<int>(id =>
                {
                    var asd = this.navigationService.Navigate<CarrierFloatingOrderDetailsViewModel, int>(id);
                });
            }
        }

        //gmaps
        public GeoPosition CurrentPosition
        {
            get
            {
                return this.currentPosition;
            }
            set
            {
                this.currentPosition = value;
                RaisePropertyChanged(() => CurrentPosition);
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


        //side view
        public MvxAsyncCommand InitSideView
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    if (this.sideViewInitialised)
                        return;

                    this.sideViewInitialised = true;
                    await this.navigationService.Navigate(this.SideView);
                });

            }
        }


        public CarrierMapViewModel(IDeviceProvider deviceProvider, IMvxNavigationService navigationService, ICarrierOrdersService ordersService, IRoutesService routesService)
        {
            this.deviceProvider = deviceProvider;
            this.navigationService = navigationService;
            this.ordersService = ordersService;
            this.routesService = routesService;

            this.ordersService.PendingOrdersUpdated += this.SendInteraction;
            this.routesService.ActiveRouteUpdated += this.SendInteraction;

            this.SideView = Mvx.IocConstruct<CarrierSideViewViewModel>();
        }

        public async override void Start()
        {
            if (initialised)
                return;

            base.Start();

            //pending orders
            await this.ordersService.GetPendingOrders();
            initialised = true;
        }

        private int? selectedSalepointId;
        private bool initialised = false;
        private float? baseZoom;
        private bool sideViewInitialised = false;
        private GeoPosition basePosition;
        private GeoPosition currentPosition = new GeoPosition();

        private IDeviceProvider deviceProvider;
        private IMvxNavigationService navigationService;
        private ICarrierOrdersService ordersService;
        private IRoutesService routesService;
    }
}
