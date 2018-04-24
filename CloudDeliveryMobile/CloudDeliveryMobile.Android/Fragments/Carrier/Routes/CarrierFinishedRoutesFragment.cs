using Android.OS;
using Android.Views;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;

namespace CloudDeliveryMobile.Android.Fragments.Carrier.Routes
{
    public class CarrierFinishedRoutesFragment : MvxFragment<CarrierFinishedRoutesViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignore = base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(FragmentId, null);
            return view;
        }

        private int FragmentId { get; } = Resource.Layout.carrier_finished_routes;
    }
}