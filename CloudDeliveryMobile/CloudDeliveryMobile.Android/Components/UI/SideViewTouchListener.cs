using Android.Views;
using Android.Views.Animations;
using System;

namespace CloudDeliveryMobile.Android.Components.UI
{
    public class SideViewTouchListener : Java.Lang.Object, View.IOnTouchListener
    {
        public SideViewTouchListener(float maxTranslation, float? autohidePosition = null, View movingView = null)
        {
            this.maxTranslation = maxTranslation;

            this.autoHidePosition = autohidePosition;
            this.movingView = movingView;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            View view = this.movingView ?? v;

            bool continueGesture = false;

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    if (Environment.TickCount - lastTouchTicks < doubleTapDuration)
                    {
                        if (view.TranslationX > autoHidePosition)
                            ShowView(view);
                        else
                            HideView(view);

                        continueGesture = false;
                    }
                    else
                    {
                        this.lastPosX = e.GetX();
                        continueGesture = true;
                    }

                    break;
                case MotionEventActions.Move:
                    float currentPosition = e.GetX();
                    float dX = lastPosX - currentPosition;

                    float transX = view.TranslationX;

                    transX -= dX;

                    //left border limit
                    if (transX < 0)
                    {
                        transX = 0;
                    }

                    //right border limit
                    if (transX > maxTranslation)
                    {
                        transX = maxTranslation;
                    }
                    view.TranslationX = transX;

                    continueGesture = true;
                    break;
                case MotionEventActions.Up:
                    if (autoHidePosition.HasValue && view.TranslationX > view.Width - autoHidePosition)
                    {
                        HideView(view);
                    }
                    lastTouchTicks = Environment.TickCount;
                    break;
                default:
                    return v.OnTouchEvent(e);
            }

            return continueGesture;
        }


        private void HideView(View v)
        {
            var intrpltr = new OvershootInterpolator(5);
            v.Animate().SetInterpolator(intrpltr)
                                      .TranslationX(maxTranslation)
                                      .SetDuration(500);
        }

        private void ShowView(View v)
        {
            var intrpltr = new DecelerateInterpolator(5);
            v.Animate().SetInterpolator(intrpltr)
                                      .TranslationX(0)
                                      .SetDuration(500);
        }


        private float lastPosX;
        private float maxTranslation;
        private float? autoHidePosition;

        private View movingView;

        private long lastTouchTicks;
        private long doubleTapDuration = 100;
    }
}