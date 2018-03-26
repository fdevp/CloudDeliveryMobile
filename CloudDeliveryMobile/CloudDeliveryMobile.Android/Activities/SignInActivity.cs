using Acr.UserDialogs;
using Android.App;
using Android.OS;
using CloudDeliveryMobile.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace CloudDeliveryMobile.Android.Activities
{
    [Activity(Label = "SigninActivity")]
    class SigninActivity : MvxAppCompatActivity<SignInViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.Theme_AppCompat_DayNight_NoActionBar);
            
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.main_signin);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}