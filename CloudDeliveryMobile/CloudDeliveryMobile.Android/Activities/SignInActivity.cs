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
using CloudDeliveryMobile.ViewModels;
using MvvmCross.Droid.Support.V7.AppCompat;

namespace CloudDeliveryMobile.Android.Activities
{
    [Activity(Label = "Logowanie")]
    class SigninActivity: MvxAppCompatActivity<SigninViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.Theme_AppCompat_DayNight_NoActionBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main_signin);

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