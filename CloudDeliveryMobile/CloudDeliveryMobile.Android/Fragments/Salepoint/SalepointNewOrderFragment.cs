using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.SalePoint;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;

namespace CloudDeliveryMobile.Android.Fragments.Salepoint
{
    [MvxFragmentPresentation(AddToBackStack = true, EnterAnimation = Resource.Drawable.animation_fade_in, PopEnterAnimation = Resource.Drawable.animation_fade_in, ExitAnimation = Resource.Drawable.animation_fade_out, PopExitAnimation = Resource.Drawable.animation_fade_out)]
    public class SalepointNewOrderFragment : MvxFragment<SalepointNewOrderViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            this.view = this.BindingInflate(FragmentId, null);
            
            Button addOrder = this.view.FindViewById<Button>(Resource.Id.salepoint_add_order_search_address);
            addOrder.Click += CreateOrderButtonClicked;
            
            if (this.geocoder == null)
                this.geocoder = new Geocoder(this.Context);

            return view;
        }

        private async void CreateOrderButtonClicked(object sender, EventArgs e)
        {
            this.ViewModel.AddressFound = false;
            this.ViewModel.GeocoderInProgress = true;
            this.ViewModel.GeocoderStarted = true;

            await Task.Run(() =>
            {
                IList<Address> addressesList = this.geocoder.GetFromLocationName(this.ViewModel.FullLocationName, 5);

                this.ViewModel.GeocoderInProgress = false;
                this.ViewModel.GeocoderFinished = true;

                if (addressesList.Count > 0)
                {
                    this.ViewModel.Model.EndLatLng.lat = addressesList[0].Latitude;
                    this.ViewModel.Model.EndLatLng.lng = addressesList[0].Longitude;
                    this.ViewModel.AddressFound = true;
                }
                else
                {
                    this.ViewModel.AddressFound = false;
                }
                
                
            });

        }


        private Geocoder geocoder;
        private View view;
        private int FragmentId { get; } = Resource.Layout.salepoint_new_order_modal;
    }
}