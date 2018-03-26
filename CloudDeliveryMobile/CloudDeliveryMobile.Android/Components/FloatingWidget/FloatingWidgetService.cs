using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components.FloatingWidget;
using CloudDeliveryMobile.ViewModels.Carrier.SideView;
using static Android.Views.View;



namespace CloudDeliveryMobile.Android.Components
{
    [Service]
    public class FloatingWidgetService : Service, IOnClickListener
    {
        public View mFloatingView;

        private IWindowManager mWindowManager;
        private View collapsedView;
        private View expandedView;
        private Binder binder;

        private CarrierSideActiveRouteViewModel viewModel;
        private FloatingWidgetExpandedController controller;
        
        public override IBinder OnBind(Intent intent)
        {
            this.binder = new FloatingWidgetBinder(this);
            return binder;
        }

        public void SetViewModel(CarrierSideActiveRouteViewModel viewModel)
        {
            this.viewModel = viewModel;
            this.viewModel.PropertyChanged += ViewModel_PropertyChanged;
            this.controller = new FloatingWidgetExpandedController(this, viewModel);
            this.controller.UpdateLayout();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            mFloatingView = LayoutInflater.From(this).Inflate(Resource.Layout.floating_widget, null, false);

            var layoutParams = new WindowManagerLayoutParams(
                ViewGroup.LayoutParams.WrapContent,
                ViewGroup.LayoutParams.WrapContent,
                WindowManagerTypes.Phone,
                WindowManagerFlags.NotFocusable,
                Format.Translucent
                );


            mWindowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
            mWindowManager.AddView(mFloatingView, layoutParams);

            collapsedView = mFloatingView.FindViewById(Resource.Id.floating_widget_layout_collapsed);
            expandedView = mFloatingView.FindViewById(Resource.Id.floating_widget_layout_expanded);

            var collapseButton = expandedView.FindViewById<RelativeLayout>(Resource.Id.widget_collapse_button);
            collapseButton.SetOnClickListener(this);

            var touchListener = new FloatingWidgetTouchListener(mWindowManager, mFloatingView, collapsedView, expandedView);
            mFloatingView.FindViewById(Resource.Id.floating_widget_layout).SetOnTouchListener(touchListener);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (mFloatingView != null)
                mWindowManager.RemoveView(mFloatingView);

            
        }

        public void OnClick(View v)
        {
            var layoutParams = (WindowManagerLayoutParams)mFloatingView.LayoutParameters;

            switch (v.Id)
            {
                case Resource.Id.widget_collapse_button:
                    //enable moving
                    layoutParams.Width = ViewGroup.LayoutParams.WrapContent;
                    mWindowManager.UpdateViewLayout(mFloatingView, layoutParams);

                    collapsedView.Visibility = ViewStates.Visible;
                    expandedView.Visibility = ViewStates.Gone;
                    break;
            }
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var asd = 5;
        }

    }
}
