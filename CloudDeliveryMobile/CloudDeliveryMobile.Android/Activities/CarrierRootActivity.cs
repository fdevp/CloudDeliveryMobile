using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using CloudDeliveryMobile.Android.Components;
using CloudDeliveryMobile.Android.Components.FloatingWidget;
using CloudDeliveryMobile.Android.Fragments.Carrier;
using CloudDeliveryMobile.Android.Fragments.Carrier.Routes;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;
using Plugin.Permissions;
using Android.Provider;
using AndroidNet = Android.Net;
using CloudDeliveryMobile.Android.Fragments.Shared;

namespace CloudDeliveryMobile.Android.Activities
{
    [Activity(Label = "CloudDelivery")]
    public class CarrierRootActivity : MvxAppCompatActivity<CarrierRootViewModel>, IFloatingWidgetActivity
    {
        public FloatingWidgetService FloatingWidgetBinder { get; set; }

        public FloatingWidgetConnection FloatingWidgetConnection { get; set; }
        public bool FloatingWidetIsBound { get; set; } = false;

        protected override void OnCreate(Bundle bundle)
        {
            SetTheme(Resource.Style.Theme_AppCompat_DayNight_NoActionBar);
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.main_root);

            var viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);


            if (viewPager != null)
            {
                var fragments = new List<MvxViewPagerFragmentInfo>();
                fragments.Add(new MvxViewPagerFragmentInfo("Mapa", typeof(CarrierMapFragment), typeof(CarrierMapViewModel)));
                fragments.Add(new MvxViewPagerFragmentInfo("Zakończone", typeof(CarrierFinishedRoutesFragment), typeof(CarrierFinishedRoutesViewModel)));
                fragments.Add(new MvxViewPagerFragmentInfo("Profil", typeof(ProfileFragment), typeof(ProfileViewModel)));
                viewPager.Adapter = new MvxCachingFragmentStatePagerAdapter(this, this.SupportFragmentManager, fragments);
                viewPager.OffscreenPageLimit = 2;
            }

            this.FloatingWidgetConnection = new FloatingWidgetConnection(this);

            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabs);
            tabLayout.SetupWithViewPager(viewPager);

        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (FloatingWidetIsBound)
            {
                this.UnbindService(FloatingWidgetConnection);
                FloatingWidetIsBound = false;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (FloatingWidetIsBound)
            {
                UnbindService(FloatingWidgetConnection);
                FloatingWidetIsBound = false;
            }
        }

        public void CreateFloatingWidget()
        {
            Intent floatingWidgetIntent = new Intent(this, typeof(FloatingWidgetService));

            if (Build.VERSION.SdkInt < BuildVersionCodes.M)
            {
                this.BindService(floatingWidgetIntent, FloatingWidgetConnection, Bind.AutoCreate);
                this.MoveTaskToBack(true);
            }
            else if (Settings.CanDrawOverlays(this))
            {
                this.BindService(floatingWidgetIntent, FloatingWidgetConnection, Bind.AutoCreate);
                this.MoveTaskToBack(true);
            }
            else
            {
                /*
                        askPermission();
                         Toast.makeText(this, "You need System Alert Window Permission to do this", Toast.LENGTH_SHORT).show();
                         */
            }
        }

        private void askPermission()
        {

            AndroidNet.Uri uri = AndroidNet.Uri.Parse("package:" + this.PackageName);
            Intent intent = new Intent(Settings.ActionManageOverlayPermission, uri);

            //this.Activity.StartActivityForResult(intent,Manifest.Permission.SystemAlertWindow);
        }

    }
}