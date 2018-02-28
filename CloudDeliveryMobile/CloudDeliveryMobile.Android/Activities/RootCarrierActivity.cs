using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Fragments.Carrier;
using CloudDeliveryMobile.Android.Fragments.Carrier.Orders;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Droid.Views.Attributes;
using Plugin.Permissions;

namespace CloudDeliveryMobile.Android.Activities
{
    [Activity(Label = "RootCarrierActivity")]
    public class RootCarrierActivity : MvxAppCompatActivity<RootCarrierViewModel>
    {
        protected override void OnCreate(Bundle bundle)
        {
            
            SetTheme(Resource.Style.Theme_AppCompat_DayNight_NoActionBar);
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.main_root);

            var viewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
            if (viewPager != null)
            {
                var fragments = new List<MvxViewPagerFragmentInfo>();
                fragments.Add(new MvxViewPagerFragmentInfo("Mapa",typeof(CarrierMapFragment),typeof(CarrierMapViewModel)));
                fragments.Add(new MvxViewPagerFragmentInfo("Zamówienia", typeof(CarrierOrdersFragment), typeof(CarrierOrdersViewModel)));
                viewPager.Adapter = new MvxFragmentPagerAdapter(this, this.SupportFragmentManager, fragments);
            }
            

            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabs);
            tabLayout.SetupWithViewPager(viewPager);
        }


        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public RootCarrierViewModel RootCarrierViewModel
        {
            get { return (RootCarrierViewModel)base.ViewModel; }
        }
    }
}