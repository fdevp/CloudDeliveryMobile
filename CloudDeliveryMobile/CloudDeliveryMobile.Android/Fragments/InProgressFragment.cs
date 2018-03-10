using Android.OS;
using Android.Views;
using CloudDeliveryMobile.ViewModels;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;

namespace CloudDeliveryMobile.Android.Fragments
{
    [MvxFragmentPresentation(AddToBackStack = true, IsCacheableFragment = true)]
    public class InProgressFragment : MvxFragment<InProgressViewModel>
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View ignore = base.OnCreateView(inflater, container, savedInstanceState);
            View view = this.BindingInflate(FragmentId, null);

            return view;
        }


        private int FragmentId { get; } = Resource.Layout.main_inprogress;


    }
}