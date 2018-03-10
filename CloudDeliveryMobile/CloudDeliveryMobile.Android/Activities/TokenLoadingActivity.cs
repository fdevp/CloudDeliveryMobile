﻿using Android.App;
using Android.OS;
using CloudDeliveryMobile.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace CloudDeliveryMobile.Android.Activities
{
    [Activity(Label = "TokenLoadingActivity")]
    public class TokenLoadingActivity : MvxAppCompatActivity<TokenSignInViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.Theme_AppCompat_DayNight_NoActionBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main_tokenloading);

            if (Intent.Extras != null)
            {
                foreach (var key in Intent.Extras.KeySet())
                {
                    var value = Intent.Extras.GetString(key);
                }
            }
        }
    }
}