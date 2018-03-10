using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components;
using CloudDeliveryMobile.Android.Components.Converters;
using CloudDeliveryMobile.Android.Components.Geolocation;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using static Android.Gms.Maps.GoogleMap;

namespace CloudDeliveryMobile.Android.Fragments.Carrier
{
    [MvxTabLayoutPresentation(TabLayoutResourceId = Resource.Id.tabs, ViewPagerResourceId = Resource.Id.viewpager, ActivityHostViewModelType = typeof(RootCarrierViewModel))]
    public class CarrierMapFragment : MvxFragment<CarrierMapViewModel>, IOnMapReadyCallback
    {
        private GoogleMap map;
        private Marker mCurrentPosition;
        private GeolocationProvider geolocationProvider;

        private ImageButton geoLocButton;

        private Dictionary<int, Marker> salepointsMarkers = new Dictionary<int, Marker>();
        private int? activeSalepointId;
        private List<Marker> ordersMarkers = new List<Marker>();

        private bool setSPMarkersAfterMapInit = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            this.view = this.BindingInflate(FragmentId, null);

            //buttons
            this.geoLocButton = view.FindViewById<ImageButton>(Resource.Id.geloc_btn);
            this.geoLocButton.Click += ToggleGeolocationWatcher;

            //geolocation
            this.geolocationProvider = new GeolocationProvider(this.Activity);

            //map
            MapFragment mapFragment = (MapFragment)this.Activity.FragmentManager.FindFragmentById(Resource.Id.carrier_gmap_fragment);
            mapFragment.GetMapAsync(this);

            //interaction binding
            var set = this.CreateBindingSet<CarrierMapFragment, CarrierMapViewModel>();
            set.Bind(this).For(v => v.OrdersUpdateInteraction).To(viewModel => viewModel.OrdersUpdateInteraction).OneWay();
            set.Apply();

            //sideview
            Task.Run(async ()=> { await this.ViewModel.InitSideView.ExecuteAsync(); });

            return view;
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
            if (setSPMarkersAfterMapInit)
            {
                this.SetSalepointsMarkers(this,null);
                setSPMarkersAfterMapInit = false;
            }
        }

        public void InitCurrentPositionMarker()
        {
            MarkerOptions options = new MarkerOptions();
            BitmapDescriptor icon = BitmapDescriptorFactory.FromResource(Resource.Drawable.carrier_marker);
            options.SetIcon(icon);

            options.SetPosition(this.map.CameraPosition.Target);
            options.SetTitle("Twoja pozycja");
            options.Visible(false);

            this.mCurrentPosition = map.AddMarker(options);
            this.mCurrentPosition.Tag = new MarkerTag { Type = MarkerType.CurrentPosition };

            var currentPositionMarkerBS = this.CreateBindingSet<CarrierMapFragment, CarrierMapViewModel>();

            //position binding
            currentPositionMarkerBS.Bind(mCurrentPosition)
                .For(v => v.Position)
                .To(vm => vm.CurrentPosition)
                .TwoWay()
                .WithConversion<GeoPositionToLatLngConverter>();

            currentPositionMarkerBS.Apply();

            UpdateCurrentPosition();
        }


        public void MarkerClickEvent(object sender, MarkerClickEventArgs e)
        {
            MarkerTag tag = (MarkerTag)e.Marker.Tag;

            this.map.AnimateCamera(CameraUpdateFactory.NewLatLng(e.Marker.Position));
    
            if (tag.Type == MarkerType.Order)
            {
                this.ViewModel.ShowOrderDetails.Execute(tag.OrderId);
            }
            else if (tag.Type == MarkerType.Salepoint)
            {
                SetOrdersMarkers(tag.SalepointId.Value);
            }

        }

        //geolocation
        public void UpdateCurrentPosition()
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

        public void ToggleGeolocationWatcher(object sender, EventArgs e)
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

        //salepoints markers
        private IMvxInteraction _ordersUpdateInteraction;
        public IMvxInteraction OrdersUpdateInteraction
        {
            get => _ordersUpdateInteraction;
            set
            {
                if (_ordersUpdateInteraction != null)
                    _ordersUpdateInteraction.Requested -= SetSalepointsMarkers;

                _ordersUpdateInteraction = value;
                _ordersUpdateInteraction.Requested += SetSalepointsMarkers;
            }
        }

        public void SetSalepointsMarkers(object sender, EventArgs e)
        {
            if (this.map == null)
            {
                setSPMarkersAfterMapInit = true;
                return;
            }
                

            //remove outdated
            foreach (var item in salepointsMarkers)
            {
                if (this.ViewModel.PendingOrders.All(x => x.SalepointId != item.Key))
                {
                    item.Value.Remove();
                    salepointsMarkers.Remove(item.Key);
                }
            }

            //add new salepoint markers
            foreach (var item in this.ViewModel.PendingOrders)
            {
                if (salepointsMarkers.ContainsKey(item.SalepointId))
                    continue;

                MarkerOptions options = new MarkerOptions();
                BitmapDescriptor salepointMarkerIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.salepoint_marker);

                options.SetIcon(salepointMarkerIcon);
                options.SetPosition(new LatLng(item.SalepointLatLng.lat, item.SalepointLatLng.lng));

                Marker marker = this.map.AddMarker(options);
                marker.Tag = new MarkerTag { Type = MarkerType.Salepoint, SalepointId = item.SalepointId };

                salepointsMarkers.Add(item.SalepointId, marker);
            }


        }

        //orders markers
        public void SetOrdersMarkers(int salepointId)
        {
            if (this.activeSalepointId.HasValue && this.activeSalepointId.Value == salepointId)
                return;

            foreach(var item in ordersMarkers)
            {
                item.Remove();
            }

            ordersMarkers.Clear();


            var salepointOrders = this.ViewModel.PendingOrders.Where(x => x.SalepointId == salepointId).ToList();

            foreach(var order in salepointOrders)
            {
                MarkerOptions options = new MarkerOptions();
                BitmapDescriptor orderMarkerIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.endpoint_marker);

                options.SetIcon(orderMarkerIcon);
                options.SetPosition(new LatLng(order.EndLatLng.lat, order.EndLatLng.lng));

                Marker marker = this.map.AddMarker(options);
                marker.Tag = new MarkerTag { Type = MarkerType.Order, SalepointId = order.SalepointId, OrderId = order.Id };

                ordersMarkers.Add(marker);
            }
            
        }

        private View view;

        private int FragmentId { get; } = Resource.Layout.carrier_map;
    }
}