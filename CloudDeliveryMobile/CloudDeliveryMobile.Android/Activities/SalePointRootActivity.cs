using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using CloudDeliveryMobile.Android.Fragments.Salepoint;
using CloudDeliveryMobile.Android.Fragments.Shared;
using CloudDeliveryMobile.ViewModels;
using CloudDeliveryMobile.ViewModels.SalePoint;
using CloudDeliveryMobile.ViewModels.SalePoint.Map;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Support.V7.AppCompat;
using System.Collections.Generic;

namespace CloudDeliveryMobile.Android.Activities
{
    [Activity(Label = "CloudDelivery")]
    class SalePointRootActivity : MvxAppCompatActivity<SalePointRootViewModel>
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
                fragments.Add(new MvxViewPagerFragmentInfo("Zamówienia", typeof(SalepointMapFragment), typeof(SalepointMapViewModel)));
                fragments.Add(new MvxViewPagerFragmentInfo("Zakończone", typeof(SalepointFinishedOrdersFragment), typeof(SalepointFinishedOrdersViewModel)));
                fragments.Add(new MvxViewPagerFragmentInfo("Profil", typeof(ProfileFragment), typeof(ProfileViewModel)));
                viewPager.Adapter = new MvxCachingFragmentStatePagerAdapter(this, this.SupportFragmentManager, fragments);
                viewPager.OffscreenPageLimit = 2;
            }

            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabs);
            tabLayout.SetupWithViewPager(viewPager);
        }
    }
}