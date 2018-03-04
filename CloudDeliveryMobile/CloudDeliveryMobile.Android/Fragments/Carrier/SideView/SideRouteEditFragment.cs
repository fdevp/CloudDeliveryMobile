using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget.Helper;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components.UI;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Droid.Views.Attributes;
using XamarinItemTouchHelper;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.SideView
{
    [MvxFragmentPresentation(fragmentContentId: Resource.Id.side_view_content, IsCacheableFragment = true)]
    public class SideRouteEditFragment : MvxFragment<CarrierSideRouteEditViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);

            var pointsList = view.FindViewById<MvxRecyclerView>(Resource.Id.edit_route_points_list);
            pointsList.ItemTemplateSelector = new EditRouteItemTemplateSelector();
            return view;
        }

        private int FragmentId { get; } = Resource.Layout.carrier_side_route_edit;
    }


}