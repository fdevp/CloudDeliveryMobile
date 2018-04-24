using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components.UI;
using CloudDeliveryMobile.ViewModels.Carrier.Routes;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Droid.Views.Attributes;
using static Android.Support.V7.Widget.RecyclerView;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.Routes
{
    [MvxFragmentPresentation(FragmentContentId = Resource.Id.finished_routes_container, AddToBackStack = true, EnterAnimation = Resource.Drawable.animation_slide_in_left, PopEnterAnimation = Resource.Drawable.animation_slide_in_left, ExitAnimation = Resource.Drawable.animation_slide_out_right, PopExitAnimation = Resource.Drawable.animation_slide_out_right)]
    public class CarrierRouteDetailsFragment : MvxFragment<CarrierRouteDetailsViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(FragmentId, null);
            var recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.carrier_route_routepoints_list);
            recyclerView.ItemTemplateSelector = new RoutePointDetailsTemplateSelector();
            recyclerView.Adapter = new RoutePointDetailsExpendableAdapter((IMvxAndroidBindingContext)this.BindingContext);
            return view;
        }

        private int FragmentId { get; } = Resource.Layout.carrier_route_details;
    }

}