using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models;
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
using System.Net.Http;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierMapViewModel : BaseViewModel
    {
        public CarrierSideViewViewModel SideView { get; set; }

        //collections
        public IMvxAsyncCommand InitializeData
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    if (initialised)
                        return;

                    initialised = true;

                    //init data
                    this.InProgress = true;
                    try
                    {
                        await this.routesService.ActiveRouteDetails();
                    }
                    catch (HttpUnprocessableEntityException) // no active route
                    {
                        await this.ordersService.GetPendingOrders();
                    }
                    catch (HttpRequestException httpException) //no connection
                    {
                        this.ErrorOccured = true;
                        this.ErrorMessage = "Problem z połączeniem z serwerem.";
                    }
                    finally
                    {
                        this.InProgress = false;
                        await this.navigationService.Navigate(this.SideView);
                    }
                });
            }
        }

        private MvxInteraction _ordersUpdateInteraction = new MvxInteraction();

        public IMvxInteraction OrdersUpdateInteraction => _ordersUpdateInteraction;

        private void SendInteraction(object sender, EventArgs e)
        {
            this._ordersUpdateInteraction?.Raise();
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


        public override void ViewDestroy(bool viewFinishing = true)
        {
            base.ViewDestroy(viewFinishing);
            this.initialised = false;
        }

        private int? selectedSalepointId;
        private bool initialised = false;
        private float? baseZoom;
        private GeoPosition basePosition;
        private GeoPosition currentPosition = new GeoPosition();

        private IDeviceProvider deviceProvider;
        private IMvxNavigationService navigationService;
        private ICarrierOrdersService ordersService;
        private IRoutesService routesService;
    }
}
