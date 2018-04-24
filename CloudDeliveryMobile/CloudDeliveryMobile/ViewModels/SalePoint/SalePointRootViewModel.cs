using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Providers;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels
{
    public class SalePointRootViewModel : BaseViewModel
    {
        public SalePointRootViewModel(ISessionProvider sessionProvider, IDeviceProvider deviceProvider, INotificationsProvider notificationsProvider)
        {
            this.deviceProvider = deviceProvider;
            this.notificationsProvider = notificationsProvider;
            this.sessionProvider = sessionProvider;

            this.notificationsProvider.SetAuthHeader(this.sessionProvider.SessionData.access_token);
            this.notificationsProvider.SetEventHandlers(Roles.salepoint);
            Task.Run(async () =>
            {
                await this.notificationsProvider.StarListening();
            });
        }

        public async override void Start()
        {
            //set root viewmodel
            this.deviceProvider.RootViewModel = this;
            /*
            this.notificationsProvider.SetAuthHeader(this.sessionProvider.SessionData.access_token);
            this.notificationsProvider.InitEvents(Roles.salepoint);
            await this.notificationsProvider.StarListening();*/
        }

        private IDeviceProvider deviceProvider;
        private INotificationsProvider notificationsProvider;
        private ISessionProvider sessionProvider;
    }
}
