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
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.SalePoint.SideView;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;

namespace CloudDeliveryMobile.Android.Fragments.Salepoint.SideView
{
    [MvxTabLayoutPresentation(TabLayoutResourceId = Resource.Id.salepoint_side_view_tabs, ViewPagerResourceId = Resource.Id.salepoint_side_view_viewpager, ActivityHostViewModelType = typeof(SalePointRootViewModel))]
    public class SalepointSideInProgressOrdersFragment : MvxFragment<SalepointSideInProgressOrdersViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(FragmentId, null);
            return view;
        }

        private int FragmentId { get; } = Resource.Layout.salepoint_side_inprogress_orders;
    }
}