using Acr.UserDialogs;
using CloudDeliveryMobile.Helpers.Exceptions;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.Models.Orders;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.ViewModels.SalePoint.SideView;
using Microsoft.AspNet.SignalR.Client;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.Core;
using Refit;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels.SalePoint.Map
{
    public class SalepointMapViewModel : BaseViewModel
    {
        public SalepointSideViewViewModel SideView { get; set; }

        public List<OrderSalepoint> InProgressOrders
        {
            get
            {
                return this.salepointOrdersService.InProgressOrders;
            }
        }

        public List<OrderSalepoint> AddedOrders
        {
            get
            {
                return this.salepointOrdersService.AddedOrders;
            }
        }


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

        //refresh data
        public bool RefreshingDataInProgress { get; set; } = false;

        public IMvxAsyncCommand RefreshData
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    this.RefreshingDataInProgress = true;
                    RaisePropertyChanged(() => this.RefreshingDataInProgress);

                    Task[] reinitTasks = { this.salepointOrdersService.GetAddedOrders(), this.salepointOrdersService.GetInProgressOrders() };

                    try
                    {
                        await Task.WhenAll(reinitTasks);
                        dialogsService.Toast("Zaktualizowano zamówienia", TimeSpan.FromSeconds(5));
                    }
                    catch (ApiException ex)
                    {
                        dialogsService.Toast("Błąd aktualizacji zamówień", TimeSpan.FromSeconds(5));
                    }
                    catch (HttpRequestException)
                    {
                        dialogsService.Toast("Problem z połączniem z serwerem", TimeSpan.FromSeconds(5));
                    }


                    this.RefreshingDataInProgress = false;
                    RaisePropertyChanged(() => this.RefreshingDataInProgress);
                });
            }
        }

        //added orders interaction
        private MvxInteraction<ServiceEvent<SalepointAddedOrdersEvents>> _addedOrdersUpdateInteraction = new MvxInteraction<ServiceEvent<SalepointAddedOrdersEvents>>();

        public IMvxInteraction<ServiceEvent<SalepointAddedOrdersEvents>> AddedOrdersUpdateInteraction => _addedOrdersUpdateInteraction;

        //inprogress orders interaction
        private MvxInteraction<ServiceEvent<SalepointInProgressOrdersEvents>> _inProgressOrdersUpdateInteraction = new MvxInteraction<ServiceEvent<SalepointInProgressOrdersEvents>>();

        public IMvxInteraction<ServiceEvent<SalepointInProgressOrdersEvents>> InProgressOrdersUpdateInteraction => _inProgressOrdersUpdateInteraction;

        //position
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

        public SalepointMapViewModel(IMvxNavigationService navigationService, ISalepointOrdersService salepointOrdersService, INotificationsProvider notificationsProvider, IUserDialogs dialogsService)
        {
            this.navigationService = navigationService;
            this.SideView = Mvx.IocConstruct<SalepointSideViewViewModel>();
            this.salepointOrdersService = salepointOrdersService;
            this.notificationsProvider = notificationsProvider;
            this.dialogsService = dialogsService;

            this.notificationsProvider.SocketStatusUpdated += (sender, value) => RaisePropertyChanged(() => this.SignalrConnectionStatus);

            this.salepointOrdersService.AddedOrdersUpdated += (sender,value) => _addedOrdersUpdateInteraction.Raise(value);
            this.salepointOrdersService.InProgressOrdersUpdated += (sender, value) => _inProgressOrdersUpdateInteraction.Raise(value);
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

        private bool sideViewInitialised = false;
        private float? baseZoom;
        private GeoPosition basePosition;
        private IMvxNavigationService navigationService;
        private ISalepointOrdersService salepointOrdersService;
        private INotificationsProvider notificationsProvider;
        private IUserDialogs dialogsService;
    }



}
