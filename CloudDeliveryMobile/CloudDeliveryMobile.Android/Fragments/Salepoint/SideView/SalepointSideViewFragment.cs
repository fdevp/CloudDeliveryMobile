using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components.UI;
using CloudDeliveryMobile.ViewModels.Carrier;
using CloudDeliveryMobile.ViewModels.SalePoint;
using CloudDeliveryMobile.ViewModels.SalePoint.SideView;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using static Android.Views.ViewTreeObserver;

namespace CloudDeliveryMobile.Android.Fragments.Salepoint.SideView
{
    [MvxFragmentPresentation(fragmentContentId: Resource.Id.salepoint_side_view_layout, IsCacheableFragment = true)]
    public class SalepointSideViewFragment : MvxFragment<SalepointSideViewViewModel>, IOnGlobalLayoutListener
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            this.view = this.BindingInflate(FragmentId, null);

            var viewPager = this.view.FindViewById<ViewPager>(Resource.Id.salepoint_side_view_viewpager);
            if (viewPager != null)
            {
                var fragments = new List<MvxViewPagerFragmentInfo>();
                fragments.Add(new MvxViewPagerFragmentInfo("Oczekujące", typeof(SalepointSideAddedOrdersFragment), typeof(SalepointSideAddedOrdersViewModel)));
                fragments.Add(new MvxViewPagerFragmentInfo("W trakcie", typeof(SalepointSideInProgressOrdersFragment), typeof(SalepointSideInProgressOrdersViewModel)));
                viewPager.Adapter = new MvxFragmentPagerAdapter(this.Context, this.ChildFragmentManager, fragments);
            }

            var tabLayout = this.view.FindViewById<TabLayout>(Resource.Id.salepoint_side_view_tabs);
            tabLayout.SetupWithViewPager(viewPager);


            this.view.ViewTreeObserver.AddOnGlobalLayoutListener(this);

            return view;
        }

        public void OnGlobalLayout()
        {
            if (sideViewInitialised)
                return;

            var containerElement = this.view.FindViewById<RelativeLayout>(Resource.Id.salepoint_side_view_container);

            var sideViewTouchElement = view.FindViewById<RelativeLayout>(Resource.Id.salepoint_side_view_touch);

            var contentElement = view.FindViewById<RelativeLayout>(Resource.Id.salepoint_side_view_content);

            this.maxTranslationX = containerElement.Width - sideViewTouchElement.Width;

            containerElement.TranslationX = maxTranslationX;

            //touch listener
            var touchListener = new SideViewTouchListener(maxTranslationX, (float)(contentElement.Width / 3), containerElement);
            sideViewTouchElement.SetOnTouchListener(touchListener);


            this.sideViewInitialised = true;
        }

        View view;

        private bool sideViewInitialised = false;
        private int maxTranslationX;
        private int FragmentId { get; } = Resource.Layout.salepoint_side_view;
    }
}