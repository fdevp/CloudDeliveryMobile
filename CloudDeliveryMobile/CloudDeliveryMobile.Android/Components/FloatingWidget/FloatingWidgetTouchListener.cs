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
using static Android.Views.View;

namespace CloudDeliveryMobile.Android.Components
{
    public class FloatingWidgetTouchListener : Java.Lang.Object, IOnTouchListener
    {
        public FloatingWidgetTouchListener(IWindowManager windowManager, View floatingView, View collapsedView, View expandedView)
        {
            this.mWindowManager = windowManager;
            this.mFloatingView = floatingView;
            this.collapsedView = collapsedView;
            this.expandedView = expandedView;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            var layoutParams = (WindowManagerLayoutParams)mFloatingView.LayoutParameters;
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    initialX = layoutParams.X;
                    initialY = layoutParams.Y;
                    initialTouchX = e.RawX;
                    initialTouchY = e.RawY;
                    return true;

                case MotionEventActions.Up:
                    int Xdiff = (int)(e.RawX - initialTouchX);
                    int Ydiff = (int)(e.RawY - initialTouchY);

                    if (Xdiff > 5 || Ydiff > 5)
                    {
                        return true;
                    }


                    if (collapsedView.Visibility == ViewStates.Visible)
                    {
                        //fix width
                        layoutParams.Width = ViewGroup.LayoutParams.MatchParent;
                        mWindowManager.UpdateViewLayout(mFloatingView, layoutParams);

                        collapsedView.Visibility = ViewStates.Gone;
                        expandedView.Visibility = ViewStates.Visible;
                    }


                    return true;
                case MotionEventActions.Move:
                    layoutParams.X = initialX + (int)(e.RawX - initialTouchX);
                    layoutParams.Y = initialY + (int)(e.RawY - initialTouchY);
                    mWindowManager.UpdateViewLayout(mFloatingView, layoutParams);
                    return true;
            }
            return false;
        }

        private IWindowManager mWindowManager;
        private View mFloatingView;
        private View collapsedView;
        private View expandedView;

        private int initialX;
        private int initialY;
        private float initialTouchX;
        private float initialTouchY;
    }
}