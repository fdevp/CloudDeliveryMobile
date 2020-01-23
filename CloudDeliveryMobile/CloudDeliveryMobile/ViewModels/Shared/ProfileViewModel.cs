using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Services;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;

namespace CloudDeliveryMobile.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {

        public string Username
        {
            get
            {
                return this.sessionProvider.SessionData.Name;
            }
        }

        public string Role
        {
            get
            {
                if (this.sessionProvider.SessionData.InRole("carrier"))
                    return "Kierowca";
                else if (this.sessionProvider.SessionData.InRole("salepoint"))
                    return "Punkt sprzedaży";

                return string.Empty;
            }
        }

        public int TodayCount
        {
            get
            {
                return 7;
            }
        }

        public int WeekCount
        {
            get
            {
                return 25;
            }
        }

        public IMvxAsyncCommand SignOut
        {
            get
            {
                return new MvxAsyncCommand(async () =>
                {
                    await this.sessionProvider.Logout();
                    this.carrierOrdersService.ClearData();
                    this.salepointOrdersService.ClearData();
                    this.routesService.ClearData();
                    this.notificationsProvider.ClearData();

                    await this.navigationService.Close(this.deviceProvider.RootViewModel).ContinueWith(async t =>
                    {
                        await this.navigationService.Navigate(typeof(SignInViewModel));
                    });
                });
            }
        }

        public ProfileViewModel(IMvxNavigationService navigationService, ISessionProvider sessionProvider, IDeviceProvider deviceProvider,
            ICarrierOrdersService carrierOrdersService, ISalepointOrdersService salepointOrdersService, IRoutesService routesService, INotificationsProvider notificationsProvider)
        {
            this.sessionProvider = sessionProvider;
            this.navigationService = navigationService;
            this.deviceProvider = deviceProvider;
            this.carrierOrdersService = carrierOrdersService;
            this.salepointOrdersService = salepointOrdersService;
            this.routesService = routesService;
            this.notificationsProvider = notificationsProvider;
        }

        private ISessionProvider sessionProvider;
        private IMvxNavigationService navigationService;
        private IDeviceProvider deviceProvider;
        private ICarrierOrdersService carrierOrdersService;
        private ISalepointOrdersService salepointOrdersService;
        private IRoutesService routesService;
        private INotificationsProvider notificationsProvider;
    }
}
