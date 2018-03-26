using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Activities;
using CloudDeliveryMobile.Android.Components.Map;
using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Models.Routes;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;

using AndroidNet = Android.Net;

namespace CloudDeliveryMobile.Android.Components.FloatingWidget
{
    public class FloatingWidgetExpandedController
    {
        private FloatingWidgetService service;
        private CarrierSideActiveRouteViewModel viewModel;
        private RoutePointActiveListViewModel currentPoint;


        //buttons
        private ImageButton nextPointButton;
        private ImageButton previousPointButton;
        private Button finishOrderButton;
        private Button showAppButton;
        private Button gmapsIntentButton;


        //labels
        private TextView pointIndex;
        private TextView pointTypeHeader;
        private ImageView pointTypeIcon;
        private TextView pointAddress;

        private TextView passedTimeText;
        private LinearLayout passedContainer;

        public FloatingWidgetExpandedController(FloatingWidgetService service, CarrierSideActiveRouteViewModel viewModel)
        {

            this.service = service;
            this.viewModel = viewModel;

            this.currentPoint = this.viewModel.Points.Where(x => x.Active).FirstOrDefault();

            var expandedView = service.mFloatingView.FindViewById(Resource.Id.floating_widget_layout_expanded);

            pointIndex = expandedView.FindViewById<TextView>(Resource.Id.widget_current_index);
            pointTypeHeader = expandedView.FindViewById<TextView>(Resource.Id.widget_point_type);
            pointTypeIcon = expandedView.FindViewById<ImageView>(Resource.Id.widget_point_type_icon);
            pointAddress = expandedView.FindViewById<TextView>(Resource.Id.widget_point_address);

            passedTimeText = expandedView.FindViewById<TextView>(Resource.Id.widget_passed_time_text);
            passedContainer = expandedView.FindViewById<LinearLayout>(Resource.Id.widget_passed_time_container);

            //buttons
            nextPointButton = expandedView.FindViewById<ImageButton>(Resource.Id.widget_next_point_button);
            nextPointButton.Click += NextPointClick;

            previousPointButton = expandedView.FindViewById<ImageButton>(Resource.Id.widget_previous_point_button);
            previousPointButton.Click += PreviousPointClick;

            finishOrderButton = expandedView.FindViewById<Button>(Resource.Id.widget_finish_order_button);
            finishOrderButton.Click += FinishOrderClick;

            showAppButton = expandedView.FindViewById<Button>(Resource.Id.widget_show_app_button);
            showAppButton.Click += ShowAppClick;

            gmapsIntentButton = expandedView.FindViewById<Button>(Resource.Id.widget_set_gmaps_button);
            gmapsIntentButton.Click += GmapsNavigationClick;
        }





        public void UpdateLayout()
        {
            if (currentPoint.Active)
                UpdateLayoutForActivePoint();
            else if (currentPoint.Point.PassedTime.HasValue)
                UpdateLayoutForPassedPoint();
            else
                UpdateLayoutForPendingPoint();

            SetLabelsText();
            UpdateSideButtons();
        }

        private void UpdateLayoutForPassedPoint()
        {
            pointTypeIcon.Visibility = ViewStates.Gone;
            pointTypeHeader.SetTextColor(ContextCompat.GetColorStateList(this.service.BaseContext, Resource.Color.grayColor));

            finishOrderButton.Visibility = ViewStates.Gone;
            showAppButton.Visibility = ViewStates.Gone;
            gmapsIntentButton.Visibility = ViewStates.Gone;

            passedContainer.Visibility = ViewStates.Visible;
            string time = currentPoint.Point.PassedTime.Value.ToString("H:mm");
            if (currentPoint.Point.Type == RoutePointType.EndPoint)
                passedTimeText.Text = string.Concat("Dostarczono: ", time);
            else if (currentPoint.Point.Type == RoutePointType.SalePoint)
                passedTimeText.Text = string.Concat("Odebrano: ", time);
        }

        private void UpdateLayoutForActivePoint()
        {
            pointTypeIcon.Visibility = ViewStates.Visible;
            pointTypeIcon.SetImageResource(Resource.Drawable.marker);
            pointTypeHeader.SetTextColor(ContextCompat.GetColorStateList(this.service.BaseContext, Resource.Color.primaryColor));

            finishOrderButton.Visibility = ViewStates.Visible;
            showAppButton.Visibility = ViewStates.Visible;
            gmapsIntentButton.Visibility = ViewStates.Visible;

            passedContainer.Visibility = ViewStates.Gone;
        }

        private void UpdateLayoutForPendingPoint()
        {
            //labels
            pointTypeIcon.Visibility = ViewStates.Visible;
            pointTypeIcon.SetImageResource(Resource.Drawable.marker_bw);
            pointTypeHeader.SetTextColor(ContextCompat.GetColorStateList(this.service.BaseContext, Resource.Color.blackColor));

            finishOrderButton.Visibility = ViewStates.Gone;
            showAppButton.Visibility = ViewStates.Visible;
            gmapsIntentButton.Visibility = ViewStates.Visible;

            passedContainer.Visibility = ViewStates.Gone;
        }

        private void SetLabelsText()
        {
            if (currentPoint.Point.Type == RoutePointType.SalePoint)
            {
                pointTypeHeader.Text = currentPoint.Point.Order.SalepointName;
                pointAddress.Text = string.Concat(currentPoint.Point.Order.SalepointCity, ", ", currentPoint.Point.Order.SalepointAddress);
            }
            else if (currentPoint.Point.Type == RoutePointType.EndPoint)
            {
                pointTypeHeader.Text = service.GetString(Resource.String.endpoint_display_name);
                pointAddress.Text = string.Concat(currentPoint.Point.Order.DestinationCity, ", ", currentPoint.Point.Order.DestinationAddress);
            }
        }


        private void UpdateSideButtons()
        {
            int index = viewModel.Points.IndexOf(currentPoint);
            int pointsCount = viewModel.Points.Count;

            if (index == pointsCount - 1)
                this.nextPointButton.Visibility = ViewStates.Gone;
            else if (index == 0)
                this.previousPointButton.Visibility = ViewStates.Gone;
            else
                this.nextPointButton.Visibility = this.previousPointButton.Visibility = ViewStates.Visible;

            pointIndex.Text = string.Concat(index + 1, "/", pointsCount);
        }

        private void ShowAppClick(object sender, EventArgs e)
        {
            Intent intent = new Intent(service.BaseContext, typeof(CarrierRootActivity));
            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.SingleTop);
            service.BaseContext.StartActivity(intent);
        }

        private void NextPointClick(object sender, EventArgs e)
        {
            int index = viewModel.Points.IndexOf(currentPoint);

            if (index == viewModel.Points.Count - 1)
                return;

            currentPoint = viewModel.Points[index + 1];
            UpdateLayout();
        }

        private void PreviousPointClick(object sender, EventArgs e)
        {
            int index = viewModel.Points.IndexOf(currentPoint);

            if (index == 0)
                return;

            currentPoint = viewModel.Points[index - 1];
            UpdateLayout();
        }

        private void FinishOrderClick(object sender, EventArgs e)
        {

        }

        private void GmapsNavigationClick(object sender, EventArgs e)
        {
            Intent intent = GmapsIntentsProvider.CreatePointIntent(this.service, currentPoint.Point);

            service.BaseContext.StartActivity(intent);
        }

    }
}