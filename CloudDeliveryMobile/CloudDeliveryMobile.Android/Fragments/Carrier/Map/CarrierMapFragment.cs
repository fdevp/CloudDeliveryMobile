using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components;
using CloudDeliveryMobile.Android.Components.Converters;
using CloudDeliveryMobile.Android.Components.Geolocation;
using CloudDeliveryMobile.Android.Fragments.Carrier.Map;
using CloudDeliveryMobile.Android.Fragments.Salepoint.Map;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Routes;
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
    [MvxTabLayoutPresentation(TabLayoutResourceId = Resource.Id.tabs, ViewPagerResourceId = Resource.Id.viewpager, ActivityHostViewModelType = typeof(CarrierRootViewModel))]
    public class CarrierMapFragment : MvxFragment<CarrierMapViewModel>, IOnMapReadyCallback
    {
        private GoogleMap map;
        private Marker mCurrentPosition;
        private GeolocationProvider geolocationProvider;

        private ImageButton geoLocButton;

        private Dictionary<int, Marker> salepointsMarkers = new Dictionary<int, Marker>();
        private Dictionary<int, Marker> ordersMarkers = new Dictionary<int, Marker>();
        private Dictionary<int, Marker> routePointsMarkers = new Dictionary<int, Marker>();

        private bool setMarkersAfterMapInitialisation = false;

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
            Task.Run(async () => { await this.ViewModel.InitSideView.ExecuteAsync(); });

            //floating label fragment
            this.carrierFloatingSalepointLabelFragment = new CarrierFloatingSalepointLabelFragment(() => { this.ViewModel.SelectedSalepointId = null; this.CloseSalepointLabelFragment(); });

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
            if (setMarkersAfterMapInitialisation)
            {
                this.SetMarkers(this, null);
                setMarkersAfterMapInitialisation = false;
            }
        }

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

        private void MarkerClickEvent(object sender, MarkerClickEventArgs e)
        {
            MarkerTag tag = (MarkerTag)e.Marker.Tag;

            this.map.AnimateCamera(CameraUpdateFactory.NewLatLng(e.Marker.Position));

            if (tag.Type == MarkerType.Order)
            {
                this.ViewModel.ShowOrderDetails.Execute(tag.OrderId.Value);
            }
            else if (tag.Type == MarkerType.Salepoint)
            {
                if (this.ViewModel.SelectedSalepointId.HasValue && this.ViewModel.SelectedSalepointId.Value == tag.SalepointId.Value)
                    return;

                this.ViewModel.SelectedSalepointId = tag.SalepointId;
                this.ShowSalepointLabelFragment();

                //new salepoint selected
                SetOrdersMarkers(true);
            }

        }

        //geolocation
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

        //salepoints markers
        private IMvxInteraction _ordersUpdateInteraction;
        public IMvxInteraction OrdersUpdateInteraction
        {
            get => _ordersUpdateInteraction;
            set
            {
                if (_ordersUpdateInteraction != null)
                    _ordersUpdateInteraction.Requested -= SetMarkers;

                _ordersUpdateInteraction = value;
                _ordersUpdateInteraction.Requested += SetMarkers;
            }
        }

        private void SetMarkers(object sender, EventArgs e)
        {
            if (this.map == null)
            {
                setMarkersAfterMapInitialisation = true;
                return;
            }

            if (this.ViewModel.IsActiveRoute)
                SetRouteMarkers();
            else
                SetSalepointsMarkers();


        }

        private void SetSalepointsMarkers()
        {
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

            //if active salepoint has not orders
            if (this.ViewModel.SelectedSalepointId.HasValue && !this.salepointsMarkers.ContainsKey(this.ViewModel.SelectedSalepointId.Value))
                this.ViewModel.SelectedSalepointId = null;

            //if has update markers
            if (this.ViewModel.SelectedSalepointId.HasValue)
                SetOrdersMarkers();
        }

        private void SetRouteMarkers()
        {
            //if active route finished
            if (this.ViewModel.ActiveRoute == null)
            {
                foreach (var item in this.routePointsMarkers)
                    item.Value.Remove();

                this.routePointsMarkers.Clear();

                return;
            }

            //if created new active route
            if (this.routePointsMarkers.Count == 0 && this.ViewModel.ActiveRoute != null)
            {

                int activeRoutePointId = this.ViewModel.ActiveRoute.Points.OrderByDescending(x => x.Index).FirstOrDefault().Id;

                foreach (RoutePoint item in this.ViewModel.ActiveRoute.Points)
                {
                    MarkerOptions options = new MarkerOptions();
                    options.SetPosition(new LatLng(item.Order.EndLatLng.lat, item.Order.EndLatLng.lng));

                    //icon
                    BitmapDescriptor orderMarkerIcon;
                    if (item.Id == activeRoutePointId)
                    {
                        orderMarkerIcon = item.Type == RoutePointType.EndPoint ? BitmapDescriptorFactory.FromResource(Resource.Drawable.endpoint_marker) : BitmapDescriptorFactory.FromResource(Resource.Drawable.salepoint_marker);
                    }
                    else
                    {
                        orderMarkerIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.marker_bw);
                    }

                    options.SetIcon(orderMarkerIcon);


                    Marker marker = this.map.AddMarker(options);
                    marker.Tag = new MarkerTag { Type = MarkerType.Order, PointId = item.Id, OrderId = item.OrderId };

                    ordersMarkers.Add(item.Id, marker);
                }

                return;
            }

            //if changes one of the point in active route
            foreach (var item in this.ViewModel.ActiveRoute.Points)
            {

            }

        }

        //orders markers
        private void SetOrdersMarkers(bool clearAll = false)
        {
            if (clearAll)
            {
                //remove all
                foreach (var item in ordersMarkers)
                {
                    item.Value.Remove();
                }

                ordersMarkers.Clear();
            }
            else
            {
                //remove outdated
                foreach (var item in ordersMarkers)
                {
                    if (this.ViewModel.PendingOrders.All(x => x.Id != item.Key))
                    {
                        item.Value.Remove();
                        ordersMarkers.Remove(item.Key);
                    }
                }
            }



            var salepointOrders = this.ViewModel.PendingOrders.Where(x => x.SalepointId == this.ViewModel.SelectedSalepointId &&
                                                                          ordersMarkers.All(y => y.Key != x.Id)).ToList();

            foreach (var order in salepointOrders)
            {
                MarkerOptions options = new MarkerOptions();
                BitmapDescriptor orderMarkerIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.endpoint_marker);

                options.SetIcon(orderMarkerIcon);
                options.SetPosition(new LatLng(order.EndLatLng.lat, order.EndLatLng.lng));

                Marker marker = this.map.AddMarker(options);
                marker.Tag = new MarkerTag { Type = MarkerType.Order, SalepointId = order.SalepointId, OrderId = order.Id };

                ordersMarkers.Add(order.Id, marker);
            }

        }


        //mvvmcross 5.6.3 navigation service has problems with closing fragments together with activity
        private void ShowSalepointLabelFragment()
        {
            var ft = this.ChildFragmentManager.BeginTransaction();
            ft.SetCustomAnimations(Resource.Drawable.animation_slide_in_left, Resource.Drawable.animation_slide_out_right);

            if (this.selectedSalepointLabelActive)
                CloseSalepointLabelFragment();

            ft.Add(carrierFloatingSalepointLabelFragment, "carrierFloatingSalepointLabelFragment");
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
        private View view;

        private int FragmentId { get; } = Resource.Layout.carrier_map;
    }
}