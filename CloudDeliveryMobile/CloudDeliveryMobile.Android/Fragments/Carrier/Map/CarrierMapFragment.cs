using System;
using System.Linq;
using System.Threading.Tasks;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using CloudDeliveryMobile.Android.Components;
using CloudDeliveryMobile.Android.Components.Geolocation;
using CloudDeliveryMobile.Android.Components.Map;
using CloudDeliveryMobile.Android.Fragments.Carrier.Map;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.Carrier;
using Microsoft.AspNet.SignalR.Client;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using MvvmCross.Platform.Core;
using static Android.Gms.Maps.GoogleMap;

namespace CloudDeliveryMobile.Android.Fragments.Carrier
{
    [MvxTabLayoutPresentation(TabLayoutResourceId = Resource.Id.tabs, ViewPagerResourceId = Resource.Id.viewpager, ActivityHostViewModelType = typeof(CarrierRootViewModel))]
    public class CarrierMapFragment : MvxFragment<CarrierMapViewModel>, IOnMapReadyCallback
    {
      
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            this.view = this.BindingInflate(FragmentId, null);

            //signalr animation
            this.SetRotationAnimation();

            //buttons
            this.geoLocButton = view.FindViewById<ImageButton>(Resource.Id.geloc_btn);
            this.geoLocButton.Click += ToggleGeolocationWatcher;

            //geolocation
            this.geolocationProvider = new GeolocationProvider(this.Activity);

            //map
            if (mapFragment == null)
            {
                mapFragment = (MapFragment)this.Activity.FragmentManager.FindFragmentById(Resource.Id.carrier_gmap_fragment);
                mapFragment.GetMapAsync(this);
            }


            //floating label fragment

            this.carrierFloatingSalepointLabelFragment = new CarrierFloatingSalepointLabelFragment(() =>
            {
                this.ViewModel.SelectedSalepointId = null;
            });

            ToggleSalepoinLabelFragment(null,null);

            this.ViewModel.SelectedSalepointUpdate.Requested += ToggleSalepoinLabelFragment;

            return view;
        }

        public override void OnStart()
        {
            base.OnStart();

            //data
            this.ViewModel.InitializeSideView.ExecuteAsync();
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            this.CloseSalepointLabelFragment();
            this.ViewModel.SelectedSalepointId = null;
            base.OnSaveInstanceState(outState);
        }

        public override void OnDestroy()
        {
            this.ViewModel.SelectedSalepointUpdate.Requested -= ToggleSalepoinLabelFragment;

            //remove markers manager interactions bindings
            if(this.mapMarkersManager != null)
            {
                this._ordersUpdateInteraction.Requested -= this.mapMarkersManager.HandleOrdersMarkers;
                this._routeUpdateInteraction.Requested -= this.mapMarkersManager.HandleRouteMarkers;
                this._selectedSalepointUpdateInteraction.Requested -= this.mapMarkersManager.HandleSelectedSalepointMarkers;
                this.mapMarkersManager.Dispose();
            }
            
            
            base.OnDestroy();
        }

        //map
        public void OnMapReady(GoogleMap googleMap)
        {
            this.map = googleMap;

            //settings
            this.map.UiSettings.ZoomControlsEnabled = true;

            //markers click event
            this.map.MarkerClick += MarkerClickEvent;

            //base location
            LatLng position = new LatLng(this.ViewModel.BasePosition.lat, this.ViewModel.BasePosition.lng);
            CameraUpdate latLngZoom = CameraUpdateFactory.NewLatLngZoom(position, this.ViewModel.BaseZoom);
            this.map.MoveCamera(latLngZoom);

            this.InitCurrentPositionMarker();

            //if orders inited before
            this.InitMarkersManager();
        }

        //geolocation
        private void InitCurrentPositionMarker()
        {
            MarkerOptions options = new MarkerOptions();
            BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.carrier_marker);
            options.SetIcon(icon);

            options.SetPosition(this.map.CameraPosition.Target);
            options.SetTitle("Twoja pozycja");
            options.Visible(false);

            this.mCurrentPosition = map.AddMarker(options);
            this.mCurrentPosition.Tag = new MarkerTag { Type = MarkerType.CurrentPosition };

            UpdateCurrentPosition();
        }

        private void UpdateCurrentPosition()
        {
            Task.Run(async () =>
            {
                var loc = await this.geolocationProvider.GetLocation();
                if (loc == null)
                    return;

                this.Activity.RunOnUiThread(() =>
                {
                    this.mCurrentPosition.Position = new LatLng(loc.Latitude, loc.Longitude);
                    this.mCurrentPosition.Visible = true;
                    this.map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(this.mCurrentPosition.Position, 14f));
                });
            });
        }

        private void ToggleGeolocationWatcher(object sender, EventArgs e)
        {
            if (this.geolocationProvider.Running)
            {
                this.geolocationProvider.StopWatcher();
                this.geoLocButton.SetImageResource(Resource.Drawable.ic_my_location_grey_700_48dp);
            }
            else
            {
                this.geolocationProvider.StartWatcher(LocationRequest.PriorityHighAccuracy, 12000, 10000, new GeoPositionCallback(this.Activity, this.map, this.mCurrentPosition));
                this.mCurrentPosition.Visible = true;
                this.geoLocButton.SetImageResource(Resource.Drawable.ic_my_location_deep_orange_400_48dp);
            }

        }

        //markers
        private void MarkerClickEvent(object sender, MarkerClickEventArgs e)
        {
            MarkerTag tag = (MarkerTag)e.Marker.Tag;

            this.map.AnimateCamera(CameraUpdateFactory.NewLatLng(e.Marker.Position));

            switch (tag.Type)
            {
                case MarkerType.Order:
                    this.ViewModel.ShowOrderDetails.Execute(tag.OrderId.Value);
                    break;
                case MarkerType.Salepoint:
                    //if selected salepoint id equals active salepoint id
                    if (this.ViewModel.SelectedSalepointId.HasValue && this.ViewModel.SelectedSalepointId.Value == tag.SalepointId.Value)
                        return;
                    this.ViewModel.SelectedSalepointId = tag.SalepointId;
                    break;
                case MarkerType.ActiveRoutePoint:
                case MarkerType.PendingRoutePoint:
                    e.Marker.ShowInfoWindow();
                    break;

            }
        }

        private IMvxInteraction<ServiceEvent<CarrierOrdersEvents>> _ordersUpdateInteraction;
        public IMvxInteraction<ServiceEvent<CarrierOrdersEvents>> OrdersUpdateInteraction
        {
            get => _ordersUpdateInteraction;
            set
            {
                if (_ordersUpdateInteraction != null)
                    _ordersUpdateInteraction.Requested -= this.mapMarkersManager.HandleOrdersMarkers;

                _ordersUpdateInteraction = value;
                _ordersUpdateInteraction.Requested += this.mapMarkersManager.HandleOrdersMarkers;
            }
        }

        private IMvxInteraction<ServiceEvent<CarrierRouteEvents>> _routeUpdateInteraction;
        public IMvxInteraction<ServiceEvent<CarrierRouteEvents>> RouteUpdateInteraction
        {
            get => _routeUpdateInteraction;
            set
            {
                if (_routeUpdateInteraction != null)
                    _routeUpdateInteraction.Requested -= this.mapMarkersManager.HandleRouteMarkers;

                _routeUpdateInteraction = value;
                _routeUpdateInteraction.Requested += this.mapMarkersManager.HandleRouteMarkers;
            }
        }

        private IMvxInteraction<int?> _selectedSalepointUpdateInteraction;
        public IMvxInteraction<int?> SelectedSalepointUpdate
        {
            get => _selectedSalepointUpdateInteraction;
            set
            {
                if (_selectedSalepointUpdateInteraction != null)
                    _selectedSalepointUpdateInteraction.Requested -= this.mapMarkersManager.HandleSelectedSalepointMarkers;

                _selectedSalepointUpdateInteraction = value;
                _selectedSalepointUpdateInteraction.Requested += this.mapMarkersManager.HandleSelectedSalepointMarkers;
            }
        }

        private void InitMarkersManager()
        {
            this.mapMarkersManager = new CarrierMapMarkersManager(this.Activity, this.map, this.ViewModel);
            var set = this.CreateBindingSet<CarrierMapFragment, CarrierMapViewModel>();
            set.Bind(this).For(v => v.OrdersUpdateInteraction).To(viewModel => viewModel.OrdersUpdateInteraction).OneWay();
            set.Bind(this).For(v => v.RouteUpdateInteraction).To(viewModel => viewModel.RouteUpdateInteraction).OneWay();
            set.Bind(this).For(v => v.SelectedSalepointUpdate).To(viewModel => viewModel.SelectedSalepointUpdate).OneWay();
            set.Apply();
        }

        //ui
        private void SetRotationAnimation()
        {
            this.signalrReconnectButton = view.FindViewById<ImageButton>(Resource.Id.signalr_reconnect_button);
            this.refreshingInProgressAnimation = AnimationUtils.LoadAnimation(this.Context, Resource.Drawable.animation_rotate);

            if (this.ViewModel.SignalrConnectionStatus == ConnectionState.Connecting || this.ViewModel.SignalrConnectionStatus == ConnectionState.Reconnecting)
                this.signalrReconnectButton.StartAnimation(this.refreshingInProgressAnimation);


            this.ViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "SignalrConnectionStatus")
                {
                    if (this.ViewModel.SignalrConnectionStatus == ConnectionState.Connecting || this.ViewModel.SignalrConnectionStatus == ConnectionState.Reconnecting)
                        this.signalrReconnectButton.StartAnimation(this.refreshingInProgressAnimation);
                    else
                        this.signalrReconnectButton.Animation = null;
                }
            };
        }

        //selected salepoint name floaing label
        //mvvmcross 5.6.3 navigation service has problems with closing fragments together with activity
        private void ToggleSalepoinLabelFragment(object sender, MvxValueEventArgs<int?> e)
        {
            if (this.ViewModel.SelectedSalepointId.HasValue)
            {
                if (selectedSalepointLabelActive)
                    CloseSalepointLabelFragment();

                ShowSalepointLabelFragment();
            }
            else
            {
                CloseSalepointLabelFragment();
            }
        }

        private void ShowSalepointLabelFragment()
        {
            var ft = this.ChildFragmentManager.BeginTransaction();
            ft.SetCustomAnimations(Resource.Drawable.animation_slide_in_left, Resource.Drawable.animation_slide_out_right);

            if (this.selectedSalepointLabelActive)
                CloseSalepointLabelFragment();

            ft.Add(Resource.Id.carrier_map_floating_detail_container, carrierFloatingSalepointLabelFragment, "carrierFloatingSalepointLabelFragment");
            ft.Commit();

            string salepointName = this.ViewModel.PendingOrders.Where(x => x.SalepointId == this.ViewModel.SelectedSalepointId).FirstOrDefault().SalepointName;
            this.carrierFloatingSalepointLabelFragment.SalepointName = salepointName;

            this.selectedSalepointLabelActive = true;
        }

        private void CloseSalepointLabelFragment()
        {
            if (!selectedSalepointLabelActive)
                return;

            var ft = this.ChildFragmentManager.BeginTransaction();
            ft.SetCustomAnimations(Resource.Drawable.animation_slide_in_left, Resource.Drawable.animation_slide_out_right);
            ft.Remove(this.carrierFloatingSalepointLabelFragment);
            ft.Commit();

            this.selectedSalepointLabelActive = false;
        }

        private bool selectedSalepointLabelActive = false;
        private CarrierFloatingSalepointLabelFragment carrierFloatingSalepointLabelFragment;

        private GoogleMap map;
        private MapFragment mapFragment;
        private Marker mCurrentPosition;
        private GeolocationProvider geolocationProvider;

        private ImageButton geoLocButton;

        private ImageButton signalrReconnectButton;
        private Animation refreshingInProgressAnimation;

        private CarrierMapMarkersManager mapMarkersManager;

        private View view;

        private int FragmentId { get; } = Resource.Layout.carrier_map;
    }
}