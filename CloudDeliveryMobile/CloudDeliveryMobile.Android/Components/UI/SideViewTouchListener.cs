﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

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

            switch (e.Action)
            {
                case MotionEventActions.Down:
                    this.lastPosX = e.GetX();
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
                    if(transX > maxTranslation)
                    {
                        transX = maxTranslation;
                    }
                    view.TranslationX = transX;
                    break;
                case MotionEventActions.Up:
                    
                    if (autoHidePosition.HasValue && view.TranslationX > view.Width - autoHidePosition)
                    {
                        var intrpltr = new OvershootInterpolator(5);
                        view.Animate().SetInterpolator(intrpltr)
                                                  .TranslationX(maxTranslation)
                                                  .SetDuration(500);
                    }
                    break;
                default:
                    return v.OnTouchEvent(e);
            }

            return true;
        }


        private float lastPosX;

        private float maxTranslation;


        private float? autoHidePosition;

        private View movingView;
    }
}