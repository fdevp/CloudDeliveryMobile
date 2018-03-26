using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;

namespace CloudDeliveryMobile.Android.Fragments.Carrier
{
    //main_content
    [MvxFragmentPresentation(AddToBackStack = true, EnterAnimation = Resource.Drawable.animation_fade_in, PopEnterAnimation = Resource.Drawable.animation_fade_in, ExitAnimation = Resource.Drawable.animation_fade_out, PopExitAnimation = Resource.Drawable.animation_fade_out)]
    public class CarrierPickupOrdersFragment : MvxFragment<CarrierPickupOrdersViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);

            return view;
        }


        private int FragmentId { get; } = Resource.Layout.carrier_pickup_orders;
    }
}