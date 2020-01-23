using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Core.Navigation;
using MvvmCross.Platform;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels
{
    public class CarrierRootViewModel : BaseViewModel
    {
        public CarrierRootViewModel(ISessionProvider sessionProvider, IDeviceProvider deviceProvide, INotificationsProvider notificationsProvider, IMvxNavigationService navigationService)
        {
            this.deviceProvider = deviceProvide;
            this.deviceProvider.RootViewModel = this;

            this.notificationsProvider = notificationsProvider;
            this.notificationsProvider.SetEventHandlers(Roles.carrier);
            Task.Run(async () =>
            {
                await this.notificationsProvider.StarListening();
            });

            this.sessionProvider = sessionProvider;
            sessionProvider.SessionExpired += async (sender, args) =>
                await navigationService.Close(this).ContinueWith(async t => await navigationService.Navigate<SignInViewModel>());
        }

        private IDeviceProvider deviceProvider;
        private INotificationsProvider notificationsProvider;
        private ISessionProvider sessionProvider;
    }
}
