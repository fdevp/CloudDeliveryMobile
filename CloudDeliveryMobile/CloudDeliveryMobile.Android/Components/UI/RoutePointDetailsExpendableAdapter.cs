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
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;

namespace CloudDeliveryMobile.Android.Components.UI
{
    public class RoutePointDetailsExpendableAdapter : MvxRecyclerAdapter
    {
        public RoutePointDetailsExpendableAdapter(IMvxAndroidBindingContext bindingContext) : base(bindingContext)
        {
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            base.OnBindViewHolder(holder, position);
            View itemview = holder.ItemView;

            LinearLayout detailsContainer;
            TextView toggleText;

            if (holder.ItemViewType == Resource.Layout.carrier_routepoint_endpoint_details)
            {
                detailsContainer = itemview.FindViewById<LinearLayout>(Resource.Id.routepoint_details_endpoint_body);
                toggleText = itemview.FindViewById<TextView>(Resource.Id.routepoint_details_endpoint_toggle_text);
            }
            else
            {
                detailsContainer = itemview.FindViewById<LinearLayout>(Resource.Id.routepoint_details_salepoint_body);
                toggleText = itemview.FindViewById<TextView>(Resource.Id.routepoint_details_salepoint_toggle_text);
            }
          
            itemview.Click += (sender, args) =>
            {
                if (detailsContainer.Visibility == ViewStates.Gone)
                {
                    detailsContainer.Visibility = ViewStates.Visible;
                    toggleText.Text = "-";
                }
                else
                {
                    detailsContainer.Visibility = ViewStates.Gone;
                    toggleText.Text = "+";
                }
            };


        }

    }
}