using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CloudDeliveryMobile.ViewModels.Carrier
{
    public class CarrierMapViewModel : BaseViewModel
    {
        private MvxInteraction _ordersUpdateInteraction = new MvxInteraction();

        public IMvxInteraction OrdersUpdateInteraction => _ordersUpdateInteraction;

        public List<Order> PendingOrders
        {
            get
            {
                return this.ordersService.PendingOrders;
            }
        }

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

        public MvxCommand ShowFloatingOrders
        {
            get
            {
                return new MvxCommand(() =>
                {
                    this._ordersUpdateInteraction.Raise();
                    this.navigationService.Navigate<CarrierFloatingOrdersViewModel>();
                });
            }
        }

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

        public CarrierSideViewViewModel SideView { get; set; }

        public MvxCommand<int> ShowOrderDetails
        {
            get
            {
                return new MvxCommand<int>(id =>
                {
                    this.navigationService.Navigate<CarrierFloatingOrderDetailsViewModel, int>(id);
                });
            }
        }

        public CarrierMapViewModel(IDeviceProvider deviceProvider, IMvxNavigationService navigationService, IOrdersService ordersService)
        {
            this.deviceProvider = deviceProvider;
            this.navigationService = navigationService;
            this.ordersService = ordersService;

            this.SideView = Mvx.IocConstruct<CarrierSideViewViewModel>();
        }

        public async override void Start()
        {
            base.Start();
            //pending orders
            this.ordersService.PendingOrdersUpdated += this.SendInteraction;
            await this.ordersService.GetPendingOrders();
        }

        private void SendInteraction(object sender, EventArgs e)
        {
            this._ordersUpdateInteraction.Raise();
        }

        private float? baseZoom;
        private bool sideViewInitialised = false;
        private GeoPosition basePosition;
        private GeoPosition currentPosition = new GeoPosition();

        private IDeviceProvider deviceProvider;
        private IMvxNavigationService navigationService;
        private IOrdersService ordersService;
    }
}
