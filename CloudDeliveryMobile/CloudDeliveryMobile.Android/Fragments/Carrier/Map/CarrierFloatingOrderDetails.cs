using System;
using Android.Animation;
using Android.OS;
using Android.Views;
using Android.Widget;
using CloudDeliveryMobile.ViewModels.Carrier;
using Com.Airbnb.Lottie;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using static Android.Animation.Animator;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.Map
{
    [MvxFragmentPresentation(fragmentContentId: Resource.Id.carrier_map_floating_detail_container, AddToBackStack = true, EnterAnimation = Resource.Drawable.animation_slide_in_up, ExitAnimation = Resource.Drawable.animation_slide_out_down, PopEnterAnimation = Resource.Drawable.animation_slide_in_up, PopExitAnimation = Resource.Drawable.animation_slide_out_down)]
    public class CarrierFloatingOrderDetails : MvxFragment<CarrierFloatingOrderDetailsViewModel>, IAnimatorListener
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);

            Button acceptButton = view.FindViewById<Button>(Resource.Id.float_details_accept_order);
            acceptButton.Click += this.AcceptOrder;

            this.successAnimation = view.FindViewById<LottieAnimationView>(Resource.Id.float_details_success);
            return view;
        }

        private async void AcceptOrder(object sender, EventArgs e)
        {
            Button acceptButton = (Button)sender;
            acceptButton.Visibility = ViewStates.Gone;
            await this.ViewModel.AcceptOrder.ExecuteAsync().ContinueWith(t =>
            {
                this.Activity.RunOnUiThread(() =>
                {
                    this.successAnimation.Visibility = ViewStates.Visible;
                    this.successAnimation.AddAnimatorListener(this);
                    this.successAnimation.PlayAnimation();
                });
            });
        }

        public async void OnAnimationEnd(Animator animation)
        {
            await this.ViewModel.CloseFragment.ExecuteAsync();
        }

        public void OnAnimationRepeat(Animator animation)
        {

        }

        public void OnAnimationStart(Animator animation)
        {

        }

        public void OnAnimationCancel(Animator animation)
        {

        }

        private LottieAnimationView successAnimation;
        private int FragmentId { get; } = Resource.Layout.carrier_floating_order_details;
    }
}