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
using Android.Views.Animations;
using Android.Widget;
using CloudDeliveryMobile.Android.Components.Geolocation;
using CloudDeliveryMobile.Android.Components.Map;
using CloudDeliveryMobile.Models;
using CloudDeliveryMobile.Models.Enums.Events;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.SalePoint.Map;
using Microsoft.AspNet.SignalR.Client;
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
        private MapFragment mapFragment;

        private ImageButton refreshButton;
        private Animation refreshingInProgressAnimation;

        private ImageButton signalrReconnectButton;

        private SalepointMapMarkersManager mapMarkersManager;


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            this.view = this.BindingInflate(FragmentId, null);

            if (mapFragment == null)
            {
                mapFragment = (MapFragment)this.Activity.FragmentManager.FindFragmentById(Resource.Id.salepoint_gmap_fragment);
                mapFragment.GetMapAsync(this);
            }


            SetRotationAnimation();

            //sideview
            Task.Run(async () => { await this.ViewModel.InitSideView.ExecuteAsync(); });

            return view;

        }


        public override void OnDestroy()
        {
            if (this.mapMarkersManager != null)
            {
                this._addedOrdersUpdateInteraction.Requested -= this.mapMarkersManager.HandleAddedOrdersMarkers;
                this._inProgressOrdersUpdateInteraction.Requested -= this.mapMarkersManager.HandleInProgressOrdersMarkers;
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

            //markers manager
            this.InitMarkersManager();
        }

        //markers
        public void MarkerClickEvent(object sender, MarkerClickEventArgs e)
        {
            MarkerTag tag = (MarkerTag)e.Marker.Tag;

            this.map.AnimateCamera(CameraUpdateFactory.NewLatLng(e.Marker.Position));

            e.Marker.ShowInfoWindow();
            //this.ViewModel.Show
        }
       
        private IMvxInteraction<ServiceEvent<SalepointAddedOrdersEvents>> _addedOrdersUpdateInteraction;
        public IMvxInteraction<ServiceEvent<SalepointAddedOrdersEvents>> AddedOrdersUpdateInteraction
        {
            get => _addedOrdersUpdateInteraction;
            set
            {
                if (_addedOrdersUpdateInteraction != null)
                    _addedOrdersUpdateInteraction.Requested -= this.mapMarkersManager.HandleAddedOrdersMarkers;

                _addedOrdersUpdateInteraction = value;
                _addedOrdersUpdateInteraction.Requested += this.mapMarkersManager.HandleAddedOrdersMarkers;
            }
        }

        private IMvxInteraction<ServiceEvent<SalepointInProgressOrdersEvents>> _inProgressOrdersUpdateInteraction;
        public IMvxInteraction<ServiceEvent<SalepointInProgressOrdersEvents>> InProgressOrdersUpdateInteraction
        {
            get => _inProgressOrdersUpdateInteraction;
            set
            {
                if (_inProgressOrdersUpdateInteraction != null)
                    _inProgressOrdersUpdateInteraction.Requested -= this.mapMarkersManager.HandleInProgressOrdersMarkers;

                _inProgressOrdersUpdateInteraction = value;
                _inProgressOrdersUpdateInteraction.Requested += this.mapMarkersManager.HandleInProgressOrdersMarkers;
            }
        }


        private void InitMarkersManager()
        {
            this.mapMarkersManager = new SalepointMapMarkersManager(this.Activity, this.map, this.ViewModel);
            var set = this.CreateBindingSet<SalepointMapFragment, SalepointMapViewModel>();
            set.Bind(this).For(v => v.AddedOrdersUpdateInteraction).To(viewModel => viewModel.AddedOrdersUpdateInteraction).OneWay();
            set.Bind(this).For(v => v.InProgressOrdersUpdateInteraction).To(viewModel => viewModel.InProgressOrdersUpdateInteraction).OneWay();
            set.Apply();
        }

        private void SetRotationAnimation()
        {
            this.refreshButton = view.FindViewById<ImageButton>(Resource.Id.salepoint_refresh_data);
            this.refreshingInProgressAnimation = AnimationUtils.LoadAnimation(this.Context, Resource.Drawable.animation_rotate);

            if (this.ViewModel.RefreshingDataInProgress)
                this.refreshButton.StartAnimation(this.refreshingInProgressAnimation);


            this.signalrReconnectButton = view.FindViewById<ImageButton>(Resource.Id.signalr_reconnect_button);
            if (this.ViewModel.SignalrConnectionStatus == ConnectionState.Connecting || this.ViewModel.SignalrConnectionStatus == ConnectionState.Reconnecting)
                this.signalrReconnectButton.StartAnimation(this.refreshingInProgressAnimation);


            this.ViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RefreshingDataInProgress")
                {
                    if (this.ViewModel.RefreshingDataInProgress)

                        this.refreshButton.StartAnimation(this.refreshingInProgressAnimation);
                    else
                        this.refreshButton.Animation = null;

                }
                else if (args.PropertyName == "SignalrConnectionStatus")
                {
                    if (this.ViewModel.SignalrConnectionStatus == ConnectionState.Connecting || this.ViewModel.SignalrConnectionStatus == ConnectionState.Reconnecting)
                        this.signalrReconnectButton.StartAnimation(this.refreshingInProgressAnimation);
                    else
                        this.signalrReconnectButton.Animation = null;
                }
            };

        }

        private View view;
        private int FragmentId { get; } = Resource.Layout.salepoint_map;
    }
}