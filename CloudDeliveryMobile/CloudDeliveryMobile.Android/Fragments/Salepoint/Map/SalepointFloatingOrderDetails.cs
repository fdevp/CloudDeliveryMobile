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
using CloudDeliveryMobile.ViewModels.SalePoint.Map;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;

namespace CloudDeliveryMobile.Android.Fragments.Salepoint.Map
{
    [MvxFragmentPresentation(fragmentContentId: Resource.Id.salepoint_map_layout, AddToBackStack = true, EnterAnimation = Resource.Drawable.animation_slide_in_up, ExitAnimation = Resource.Drawable.animation_slide_out_down, PopEnterAnimation = Resource.Drawable.animation_slide_in_up, PopExitAnimation = Resource.Drawable.animation_slide_out_down)]
    public class SalepointFloatingOrderDetails : MvxFragment<SalepointFloatingOrderDetailsViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);
            return view;
        }

        private int FragmentId { get; } = Resource.Layout.carrier_floating_order_details;
    }
}