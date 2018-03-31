using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Platform;

namespace CloudDeliveryMobile.ViewModels
{
    public class CarrierRootViewModel : BaseViewModel
    {

        public CarrierRootViewModel(IDeviceProvider deviceProvide)
        {
            this.deviceProvider = deviceProvide;
        }

        public override void Start()
        {
            //set root viewmodel
            this.deviceProvider.RootViewModel = this;
        }

        private IDeviceProvider deviceProvider;
    }
}
