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
using Android.Graphics;
using Android.OS;
using Android.Runtime;
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
using MvvmCross.Droid;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using MvvmCross.Platform.Core;
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
            View view = this.BindingInflate(FragmentId, null);

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

            return view;
        }


        //map
        public void OnMapReady(GoogleMap googleMap)
        {
            this.map = googleMap;

            //settings
            this.map.UiSettings.ZoomControlsEnabled = true;

            string polyline = "ypz~HcbifB`D{VrI~OxA`\\fA~AANHVb@d@tEtCbFkPbKcyAlV{wAlX{cAr`@ai@n~AarBh\\gPfpAi[xm@kHiEzEpD}fAzd@oaFdmAczIdxA}~Gp[_yC}Dk`DkJ{`Gr@ggKT{wHuLscEkYgyCePg~EnAyhD|GyvHf[shDtRiuDrO_hCpa@guCnbAsuGndB{eGpg@akCn\\kpEjm@ybHdeAmbI~pAurGp~@{{G|eAg_H`l@k}Dhj@}eCxTqlB`h@ulCx[ilCzeA}kInYopCxQ_bCzYs~GrZmdBn[}rBpR}pCv[gdC|C_nDsKw~DwUqjD}e@seFDyuAbL{aBjn@s`FhGskCgCguFtJqiCla@c}C`iA{rHxl@awGddBihFzn@ciBpxA}vBduAk`D|hCupEto@}uB~e@{lDjt@qnD`x@axE~ZipAha@{cCpSkoCnXopBfk@mdB~jAemC`i@yaCvh@unBln@}nD|h@ufCx^epCxOsyHvw@gnI|JkaCr_@k`FrKw`HhXkpB|f@gkBho@scCxToaDkAmpDjMssC`f@svElEgzAsHk}CeOypEmRosBaHyhDuRmdC{i@giCooBctG}`AanD{yAsvD_t@cz@o_BgbD}{AgvCmmAoaGm{@k~BcaAiuDy}Ay{Eyq@afE}bAcgNmdBiuL_PcdElN}tHkBacC}L_aB}b@_cCiZkaA}b@g{Aaf@soEgxAc_Ls~@apHe}@cnEe{AwqLwn@qvJuu@whH}t@q~Ckr@qeB}`@cWoUih@kLiqAiH}zAkKwjAu]a}AnAmfAxbAajB~~@e}ChVihBlOmXuFcVce@vA_~@_VibBarCc{@ozAkHcZwHvBar@~MuLkEaCq_AeGsvAlB_J`GDrFqMhJeGjKrf@na@bnBti@lwAb{@~xAvo@ngAxXde@fRxFtp@hRtQvT?|Y}Jz{@mO~{@qd@~{AgYv|@ea@lp@{[ht@bIvoAb_@~nBdD~fAlHpdAnTd{At}@xz@|k@jjBho@f_Dh{@jqIzm@lsJnlBppMraAlrF`k@`fFp}@rgGdXrfD~Z|bCrWlbAfbA`{DhUbkCb@peDqLtzFfUltFd|@n|Fhp@~eFlm@|gJhr@~dF|nAn|DnkAhtEpw@`lBbnAdcGzd@zuAriBxbDnnAx{BnlAzoBn|BpbIfuB`~Gr_@hbCnNx~Dl]b|FtP~vFkCxz@`j@ba@re@d\\hcBrgAtvBrw@lfCvqHfM`Sbv@dSv_@dk@bYhi@bj@_C~]lYre@tGjMjLxNhQtVmExc@uF";

            
            List<LatLng> lines = PolylineDecoder.DecodePolyline(polyline);
            PolylineOptions popts = new PolylineOptions();
            foreach (var item in lines)
                popts.Add(item);

            popts.InvokeColor(Color.Green);
            popts.InvokeWidth(5);

            this.map.AddPolyline(popts);


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
                if (!this.ViewModel.PendingOrders.Any(x => x.SalepointId == item.Key))
                {
                    item.Value.Remove();
                    salepointsMarkers.Remove(item.Key);
                }
            }

            //add new orders
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

        private int FragmentId { get; } = Resource.Layout.carrier_map;
    }
}