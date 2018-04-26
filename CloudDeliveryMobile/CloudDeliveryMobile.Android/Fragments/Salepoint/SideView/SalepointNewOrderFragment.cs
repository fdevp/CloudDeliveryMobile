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
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.SalePoint;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;

namespace CloudDeliveryMobile.Android.Fragments.Salepoint
{
    [MvxFragmentPresentation(FragmentContentId = Resource.Id.salepoint_side_view_content, AddToBackStack = true, EnterAnimation = Resource.Drawable.animation_slide_in_up, PopEnterAnimation = Resource.Drawable.animation_slide_in_up, ExitAnimation = Resource.Drawable.animation_slide_out_down, PopExitAnimation = Resource.Drawable.animation_slide_out_down)]
    public class SalepointNewOrderFragment : MvxFragment<SalepointNewOrderViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            this.view = this.BindingInflate(FragmentId,container,false);

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

            IList<Address> addressesList = null;

            if (this.ViewModel.Model.DestinationCity.Length < 3 || this.ViewModel.Model.DestinationAddress.Length < 3)
            {
                Snackbar.Make(this.view, "Podaj poprawne miasto i adres", 5000).Show();

                this.ViewModel.GeocoderInProgress = false;
                this.ViewModel.GeocoderStarted = false;
                return;
            }


            await Task.Run(() =>
            {
                try
                {
                    addressesList = this.geocoder.GetFromLocationName(this.ViewModel.FullLocationName, 5);
                }
                catch (Exception ex)
                {
                    Snackbar.Make(this.view, "Wyszukiwanie wymaga połączenia z internetem", 5000).Show();
                }


                this.ViewModel.GeocoderInProgress = false;
                this.ViewModel.GeocoderFinished = true;

                if (addressesList != null && addressesList.Count > 0)
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
        private int FragmentId { get; } = Resource.Layout.salepoint_side_new_order;
    }
}