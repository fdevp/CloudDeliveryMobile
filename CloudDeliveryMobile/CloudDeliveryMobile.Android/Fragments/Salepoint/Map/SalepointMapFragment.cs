using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components.Geolocation;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.SalePoint.Map;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using static Android.Gms.Maps.GoogleMap;

namespace CloudDeliveryMobile.Android.Fragments.Salepoint
{
    [MvxTabLayoutPresentation(TabLayoutResourceId = Resource.Id.tabs, ViewPagerResourceId = Resource.Id.viewpager, ActivityHostViewModelType = typeof(SalePointRootViewModel), IsCacheableFragment = true)]
    public class SalepointMapFragment : MvxFragment<SalepointMapViewModel>, IOnMapReadyCallback
    {
        private GoogleMap map;


        private Dictionary<int, Marker> addedOrdersMarkers = new Dictionary<int, Marker>();
        private Dictionary<int, Marker> inprogressOrdersMarkers = new Dictionary<int, Marker>();

        private bool setMarkersAfterMapInitialisation = false;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            this.view = this.BindingInflate(FragmentId, null);
            MapFragment mapFragment = (MapFragment)this.Activity.FragmentManager.FindFragmentById(Resource.Id.salepoint_gmap_fragment);
            mapFragment.GetMapAsync(this);

            //sideview
            Task.Run(async () => { await this.ViewModel.InitSideView.ExecuteAsync(); });

            return view;

        }

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

            //interaction binding
            var set = this.CreateBindingSet<SalepointMapFragment, SalepointMapViewModel>();
            set.Bind(this).For(v => v.AddedOrdersUpdateInteraction).To(viewModel => viewModel.AddedOrdersUpdateInteraction).OneWay();
            set.Bind(this).For(v => v.InProgressOrdersUpdateInteraction).To(viewModel => viewModel.InProgressOrdersUpdateInteraction).OneWay();
            set.Apply();


            //if orders inited before
            if (setMarkersAfterMapInitialisation)
            {
                this.SetAddedMarkers(this, null);
                this.SetInProgressMarkers(this, null);
                setMarkersAfterMapInitialisation = false;
            }
        }

        public void MarkerClickEvent(object sender, MarkerClickEventArgs e)
        {
            MarkerTag tag = (MarkerTag)e.Marker.Tag;

            this.map.AnimateCamera(CameraUpdateFactory.NewLatLng(e.Marker.Position));

            //this.ViewModel.Show
        }


        //added orders markers
        private IMvxInteraction _addedOrdersUpdateInteraction;
        public IMvxInteraction AddedOrdersUpdateInteraction
        {
            get => _addedOrdersUpdateInteraction;
            set
            {
                if (_addedOrdersUpdateInteraction != null)
                    _addedOrdersUpdateInteraction.Requested -= SetAddedMarkers;

                _addedOrdersUpdateInteraction = value;
                _addedOrdersUpdateInteraction.Requested += SetAddedMarkers;
            }
        }

        //inprogress orders markers
        private IMvxInteraction _inProgressOrdersUpdateInteraction;
        public IMvxInteraction InProgressOrdersUpdateInteraction
        {
            get => _inProgressOrdersUpdateInteraction;
            set
            {
                if (_inProgressOrdersUpdateInteraction != null)
                    _inProgressOrdersUpdateInteraction.Requested -= SetInProgressMarkers;

                _inProgressOrdersUpdateInteraction = value;
                _inProgressOrdersUpdateInteraction.Requested += SetInProgressMarkers;
            }
        }

        private void SetAddedMarkers(object sender, EventArgs e)
        {
            if (this.map == null)
            {
                setMarkersAfterMapInitialisation = true;
                return;
            }

            //remove outdated
            foreach (var item in addedOrdersMarkers)
            {
                if (this.ViewModel.AddedOrders.All(x => x.Id != item.Key))
                {
                    item.Value.Remove();
                    addedOrdersMarkers.Remove(item.Key);
                }
            }

            //add new
            foreach (var item in this.ViewModel.AddedOrders)
            {
                if (addedOrdersMarkers.ContainsKey(item.Id))
                    continue;

                if (item.EndLatLng == null)
                    continue;

                MarkerOptions options = new MarkerOptions();
                BitmapDescriptor salepointMarkerIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.marker_bw);

                options.SetIcon(salepointMarkerIcon);
                options.SetPosition(new LatLng(item.EndLatLng.lat, item.EndLatLng.lng));

                Marker marker = this.map.AddMarker(options);
                marker.Tag = new MarkerTag { Type = MarkerType.AddedOrder, OrderId = item.Id };

                addedOrdersMarkers.Add(item.Id, marker);
            }

        }

        private void SetInProgressMarkers(object sender, EventArgs e)
        {
            if (this.map == null)
            {
                setMarkersAfterMapInitialisation = true;
                return;
            }

            //remove outdated
            foreach (var item in inprogressOrdersMarkers)
            {
                if (this.ViewModel.InProgressOrders.All(x => x.Id != item.Key))
                {
                    item.Value.Remove();
                    inprogressOrdersMarkers.Remove(item.Key);
                }
            }

            //add new
            foreach(var item in this.ViewModel.InProgressOrders)
            {
                if (inprogressOrdersMarkers.ContainsKey(item.Id))
                    continue;

                if (item.EndLatLng == null)
                    continue;

                MarkerOptions options = new MarkerOptions();
                BitmapDescriptor salepointMarkerIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.marker);

                options.SetIcon(salepointMarkerIcon);
                options.SetPosition(new LatLng(item.EndLatLng.lat, item.EndLatLng.lng));

                Marker marker = this.map.AddMarker(options);
                marker.Tag = new MarkerTag { Type = MarkerType.InProgressOrder, OrderId = item.Id };

                inprogressOrdersMarkers.Add(item.Id, marker);
            }
        }

        private View view;
        private int FragmentId { get; } = Resource.Layout.salepoint_map;
    }
}