using Android.OS;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.Android.Components.UI;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using static Android.Views.ViewTreeObserver;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.SideView
{
    [MvxFragmentPresentation(fragmentContentId: Resource.Id.side_view_layout, IsCacheableFragment = true)]
    public class SideViewFragment : MvxFragment<CarrierSideViewViewModel>, IOnGlobalLayoutListener
    {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {

            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            this.view = this.BindingInflate(FragmentId, null);

            this.view.ViewTreeObserver.AddOnGlobalLayoutListener(this);

            return view;
        }

        public void OnGlobalLayout()
        {
            if (sideViewInitialised)
                return;

            var containerElement = this.view.FindViewById<RelativeLayout>(Resource.Id.side_view_container);

            var sideViewTouchElement = view.FindViewById<RelativeLayout>(Resource.Id.side_view_touch);

            var contentElement = view.FindViewById<RelativeLayout>(Resource.Id.side_view_content);

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
        private int FragmentId { get; } = Resource.Layout.carrier_side_view;
    }
}