using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.SalePoint;
using CloudDeliveryMobile.ViewModels.SalePoint.Map;
using CloudDeliveryMobile.ViewModels.SalePoint.Orders;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;

namespace CloudDeliveryMobile.Android.Fragments.Salepoint
{
    //kopia z backupem
    //dodano id finished_orders_container
    // zmiana FragmentContentId w TYM fragmencie

    [MvxFragmentPresentation(FragmentContentId = Resource.Id.finished_orders_container, AddToBackStack = true, EnterAnimation = Resource.Drawable.animation_slide_in_left, PopEnterAnimation = Resource.Drawable.animation_slide_in_left, ExitAnimation = Resource.Drawable.animation_slide_out_right, PopExitAnimation = Resource.Drawable.animation_slide_out_right)]
    public class SalepointOrderDetailsFragment : MvxFragment<SalepointOrderDetailsViewModel>, IOnMapReadyCallback
    {
      
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(FragmentId, null);


            this.mapContainer = (LinearLayout)view.FindViewById(Resource.Id.salepoint_order_details_gmap_container);

            //interaction binding
            var set = this.CreateBindingSet<SalepointOrderDetailsFragment, SalepointOrderDetailsViewModel>();
            set.Bind(this).For(v => v.OrderReadyInteraction).To(viewModel => viewModel.OrderReadyInteraction).OneWay();
            set.Bind(this).For(v => v.OrderReadyInteraction).To(viewModel => viewModel.OrderReadyInteraction).OneWay();
            set.Apply();

            if (mapSupportFragment == null)
            {
                mapSupportFragment = (SupportMapFragment)this.ChildFragmentManager.FindFragmentById(Resource.Id.salepoint_order_details_gmap_fragment);
                mapSupportFragment.GetMapAsync(this);
            }
            else if (this.map == null)
            {
                mapSupportFragment.GetMapAsync(this);
            }



            return view;
        }

        public override void OnDestroyView()
        {
            base.OnDestroyView();
        }

        public void OnMapReady(GoogleMap googleMap)
        {

            this.map = googleMap;
            //settings
            this.map.UiSettings.ZoomControlsEnabled = true;

            if (setMarkersAfterMapInitialisation)
            {
                setMarkersAfterMapInitialisation = false;
                SetAddedMarkers(this, null);
            }
        }

        //added orders markers
        private IMvxInteraction _orderReadyInteraction;
        public IMvxInteraction OrderReadyInteraction
        {
            get => _orderReadyInteraction;
            set
            {
                if (_orderReadyInteraction != null)
                    _orderReadyInteraction.Requested -= SetAddedMarkers;

                _orderReadyInteraction = value;
                _orderReadyInteraction.Requested += SetAddedMarkers;
            }
        }

        private void SetAddedMarkers(object sender, EventArgs e)
        {
            if (this.ViewModel.Order.EndLatLng == null || this.Activity == null)
                return;

            if (this.map == null)
            {
                setMarkersAfterMapInitialisation = true;
                return;
            }

            MarkerOptions salepointMarkerOptions = new MarkerOptions();
            BitmapDescriptor salepointMarkerIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.salepoint_marker);
            salepointMarkerOptions.SetIcon(salepointMarkerIcon);
            salepointMarkerOptions.SetPosition(new LatLng(this.ViewModel.Order.SalepointLatLng.lat, this.ViewModel.Order.SalepointLatLng.lng));



            MarkerOptions endpointMarkerOptions = new MarkerOptions();
            BitmapDescriptor endpointtMarkerIcon = BitmapDescriptorFactory.FromResource(Resource.Drawable.endpoint_marker);
            endpointMarkerOptions.SetIcon(endpointtMarkerIcon);
            endpointMarkerOptions.SetPosition(new LatLng(this.ViewModel.Order.EndLatLng.lat, this.ViewModel.Order.EndLatLng.lng));

            this.Activity.RunOnUiThread(() =>
            {
                //if user close fragment before markers init
                if (this.map == null || this.mapContainer == null)
                    return;

                this.map.AddMarker(salepointMarkerOptions);
                this.map.AddMarker(endpointMarkerOptions);
                this.map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(new CameraPosition(endpointMarkerOptions.Position, 11, 0, 0)));
                this.mapContainer.Visibility = ViewStates.Visible;
            });
        }

        private bool setMarkersAfterMapInitialisation = false;
        private GoogleMap map;
        private SupportMapFragment mapSupportFragment;
        private LinearLayout mapContainer;

        private int FragmentId { get; } = Resource.Layout.salepoint_order_details;
    }
}