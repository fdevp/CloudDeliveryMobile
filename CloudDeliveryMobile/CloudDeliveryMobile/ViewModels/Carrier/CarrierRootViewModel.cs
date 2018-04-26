using CloudDeliveryMobile.Models.Enums;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.ViewModels.Carrier;
using MvvmCross.Platform;
using System.Threading.Tasks;

namespace CloudDeliveryMobile.ViewModels
{
    public class CarrierRootViewModel : BaseViewModel
    {

        public CarrierRootViewModel(ISessionProvider sessionProvider, IDeviceProvider deviceProvide, INotificationsProvider notificationsProvider)
        {
            this.deviceProvider = deviceProvide;
            this.notificationsProvider = notificationsProvider;
            this.sessionProvider = sessionProvider;

            this.notificationsProvider.SetAuthHeader(this.sessionProvider.SessionData.access_token);
            this.notificationsProvider.SetEventHandlers(Roles.carrier);

            Task.Run(async () =>
            {
                await this.notificationsProvider.StarListening();
            });
        }

        private IDeviceProvider deviceProvider;
        private INotificationsProvider notificationsProvider;
        private ISessionProvider sessionProvider;
    }
}
