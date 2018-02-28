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
using CloudDeliveryMobile.Android.Activities;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.Map
{
    [MvxFragmentPresentation(fragmentContentId: Resource.Id.carrier_map_layout, AddToBackStack = true, IsCacheableFragment = true)]
    public class CarrierFloatingOrdersFragment : MvxFragment<CarrierFloatingOrdersViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);

            return view;
        }


        private int FragmentId { get; } = Resource.Layout.carrier_floating_orders;
    }
}