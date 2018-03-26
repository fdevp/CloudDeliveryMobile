using Android;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components.Map;
using CloudDeliveryMobile.Android.Components.UI;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Droid.Views.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using AndroidNet = Android.Net;
using Android.Provider;
using CloudDeliveryMobile.Android.Components;
using CloudDeliveryMobile.Android.Components.FloatingWidget;
using CloudDeliveryMobile.Android.Activities;
using MvvmCross.Core.ViewModels;
using Android.Support.Design.Widget;
using System.Linq;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.SideView
{
    [MvxFragmentPresentation(fragmentContentId: Resource.Id.side_view_content, IsCacheableFragment = true)]
    public class SideActiveRouteFragment : MvxFragment<CarrierSideActiveRouteViewModel>
    {
        private MvxRecyclerView recyclerView;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);


            this.recyclerView = view.FindViewById<MvxRecyclerView>(Resource.Id.active_route_points_list);
            this.recyclerView.ItemTemplateSelector = new ActiveRouteItemTemplateSelector();


            //buttons
            Button navigationBtn = view.FindViewById<Button>(Resource.Id.active_route_start_navigation);
            navigationBtn.Click += OpenNavigation;

            Button minimizeBtn = view.FindViewById<Button>(Resource.Id.active_route_minimize);
            minimizeBtn.Click += MinimizeApplication;

            return view;
        }

        public void OpenNavigation(object sender, EventArgs e)
        {
            RoutePointActiveListViewModel activePoint = this.ViewModel.Points.FirstOrDefault(x => x.Active);
            if (activePoint == null)
                return;

            this.SetFloatingWidget();

            Intent intent = GmapsIntentsProvider.CreatePointIntent(this.Context, activePoint.Point);
            this.StartActivity(intent);
        }

        public void MinimizeApplication(object sender, EventArgs e)
        {
            this.SetFloatingWidget();
        }

        private void SetFloatingWidget()
        {
            var activity = this.Activity as IFloatingWidgetActivity;
            if (activity == null)
                return;

            activity.FloatingWidgetConnection.ServiceConnected += InjectViewModelToWidget;
            activity.CreateFloatingWidget();
        }

        private void InjectViewModelToWidget(object sender, EventArgs e)
        {
            var activity = this.Activity as CarrierRootActivity;
            activity.FloatingWidgetBinder.SetViewModel(this.ViewModel);
        }

        private int FragmentId { get; } = Resource.Layout.carrier_side_active_route;
    }
}