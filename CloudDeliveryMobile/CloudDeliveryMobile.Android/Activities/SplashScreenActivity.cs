using Android.App;
using Android.Content.PM;
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