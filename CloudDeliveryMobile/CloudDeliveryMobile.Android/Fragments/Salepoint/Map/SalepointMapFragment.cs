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
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using static Android.Gms.Maps.GoogleMap;

namespace CloudDeliveryMobile.Android.Fragments.Salepoint
{
    [MvxTabLayoutPresentation(TabLayoutResourceId = Resource.Id.tabs, ViewPagerResourceId = Resource.Id.viewpager, ActivityHostViewModelType = typeof(SalePointRootViewModel), IsCacheableFragment = true)]
    public class SalepointMapFragment : MvxFragment<SalepointMapViewModel>, IOnMapReadyCallback
    {
        private GoogleMap map;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View igonre = base.OnCreateView(inflater, container, savedInstanceState);
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


        }

        public void MarkerClickEvent(object sender, MarkerClickEventArgs e)
        {
            MarkerTag tag = (MarkerTag)e.Marker.Tag;

            this.map.AnimateCamera(CameraUpdateFactory.NewLatLng(e.Marker.Position));

            //this.ViewModel.Show
        }

        private View view;

        private int FragmentId { get; } = Resource.Layout.salepoint_map;
    }
}