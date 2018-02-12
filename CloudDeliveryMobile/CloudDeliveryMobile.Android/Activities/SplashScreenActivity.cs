using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmCross.Droid.Views;

namespace CloudDeliveryMobile.Android.Activities
{
    [Activity(
       MainLauncher = true
       ,Icon = "@drawable/icon"
        ,Theme = "@style/Theme.Splash"
       , NoHistory = true
       , ScreenOrientation = ScreenOrientation.Portrait)]
    class SplashScreenActivity : MvxSplashScreenActivity
    {
        public SplashScreenActivity() : base(Resource.Layout.main_splash) { }
    }
}