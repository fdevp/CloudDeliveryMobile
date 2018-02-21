using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components;
using CloudDeliveryMobile.Android.Components.Converters;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid;
using MvvmCross.Droid.Support.V4;

namespace CloudDeliveryMobile.Android.Fragments.Carrier
{
    public class CarrierMapFragment : MvxFragment<CarrierMapViewModel>, IOnMapReadyCallback
    {

        private GoogleMap map;
        private Marker mCurrentPosition;
        private GeolocationProvider geolocationProvider;

        private ImageButton geoLocButton;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);

            //buttons
            this.geoLocButton = view.FindViewById<ImageButton>(Resource.Id.gelocBtn);
            this.geoLocButton.Click += ToggleGeolocationWatcher;

            //geolocation
            this.geolocationProvider = new GeolocationProvider(this.Activity);

            //map
            MapFragment mapFragment = (MapFragment)this.Activity.FragmentManager.FindFragmentById(Resource.Id.carrier_gmap_fragment);
            mapFragment.GetMapAsync(this);

            return view;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            this.map = googleMap;

            //base location
            LatLng position = new LatLng(this.ViewModel.BasePosition.lat, this.ViewModel.BasePosition.lng);
            CameraUpdate latLngZoom = CameraUpdateFactory.NewLatLngZoom(position, this.ViewModel.BaseZoom);
            this.map.MoveCamera(latLngZoom);

            this.InitCurrentPositionMarker();
        }

        public void InitCurrentPositionMarker()
        {
            MarkerOptions options = new MarkerOptions();
            options.SetPosition(this.map.CameraPosition.Target);
            options.SetTitle("Twoja pozycja");
            options.Visible(false);
            this.mCurrentPosition = map.AddMarker(options);

            var currentPositionMarkerBS = this.CreateBindingSet<CarrierMapFragment, CarrierMapViewModel>();

            //position binding
            currentPositionMarkerBS.Bind(mCurrentPosition)
                .For(v => v.Position)
                .To(vm => vm.CurrentPosition)
                .WithConversion<GeoPositionToLatLngConverter>();

            currentPositionMarkerBS.Apply();



            //get current position
            this.Activity.RunOnUiThread(async () =>
            {
                var loc = await this.geolocationProvider.GetLocation();
                if (loc == null)
                    return;
                this.mCurrentPosition.Position = new LatLng(loc.Latitude, loc.Longitude);
                this.mCurrentPosition.Visible = true;
                this.map.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(this.mCurrentPosition.Position, 14f));
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
                this.geoLocButton.SetImageResource(Resource.Drawable.ic_my_location_deep_orange_400_48dp);
            }

        }


        private int FragmentId { get; } = Resource.Layout.carrier_map;
    }
}