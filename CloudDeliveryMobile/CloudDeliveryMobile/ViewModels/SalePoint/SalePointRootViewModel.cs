using CloudDeliveryMobile.Providers;

namespace CloudDeliveryMobile.ViewModels
{
    public class SalePointRootViewModel : BaseViewModel
    {
        public SalePointRootViewModel(IDeviceProvider deviceProvider)
        {
            this.deviceProvider = deviceProvider;
        }

        public override void Start()
        {
            //set root viewmodel
            this.deviceProvider.RootViewModel = this;
        }

        private IDeviceProvider deviceProvider;
    }
}
