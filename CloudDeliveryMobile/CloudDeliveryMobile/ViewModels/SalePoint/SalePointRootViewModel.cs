using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Providers;
using MvvmCross.Core.Navigation;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels
{
    public class SalePointRootViewModel : BaseViewModel
    {
        public SalePointRootViewModel(ISessionProvider sessionProvider, IDeviceProvider deviceProvider, INotificationsProvider notificationsProvider, IMvxNavigationService navigationService)
        {
            this.deviceProvider = deviceProvider;
            this.deviceProvider.RootViewModel = this;

            this.notificationsProvider = notificationsProvider;
            this.notificationsProvider.SetEventHandlers(Roles.salepoint);
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
