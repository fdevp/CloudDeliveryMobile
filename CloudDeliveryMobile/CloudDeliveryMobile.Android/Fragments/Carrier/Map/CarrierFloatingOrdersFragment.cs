using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using static Android.Resource;
using Animation = Android.Views.Animations.Animation;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.Map
{
    [MvxFragmentPresentation(fragmentContentId: Resource.Id.carrier_map_floating_list_container, AddToBackStack = true, IsCacheableFragment = true)]
    public class CarrierFloatingOrdersFragment : MvxFragment<CarrierFloatingOrdersViewModel>
    {
        private ImageButton refreshButton;
        private Animation refreshingInProgressAnimation;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);

            this.refreshButton = view.FindViewById<ImageButton>(Resource.Id.carrier_refresh_pending_orders);

            this.refreshingInProgressAnimation = AnimationUtils.LoadAnimation(this.Context, Resource.Drawable.animation_rotate);
            if (this.ViewModel.RefreshingDataInProgress)
                this.refreshButton.StartAnimation(this.refreshingInProgressAnimation);

            this.ViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "RefreshingDataInProgress")
                {
                    if (this.ViewModel.RefreshingDataInProgress)
                    {
                        this.refreshButton.StartAnimation(this.refreshingInProgressAnimation);
                    }
                    else
                    {
                        this.refreshButton.Animation = null;
                    }
                }
            };

            return view;
        }


        private int FragmentId { get; } = Resource.Layout.carrier_floating_orders;
    }
}