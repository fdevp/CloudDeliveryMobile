using Acr.UserDialogs;
using CloudDeliveryMobile.ApiInterfaces;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Providers.Implementations;
using CloudDeliveryMobile.Resources;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.Services.Implementations;
using CloudDeliveryMobile.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using PCLStorage;
using Refit;

namespace CloudDeliveryMobile
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {

            var deviceProvider = new DeviceProvider();
            Mvx.RegisterSingleton<IDeviceProvider>(deviceProvider);

            var notificationsProvider = new NotificationsProvider();
            Mvx.RegisterSingleton<INotificationsProvider>(notificationsProvider);

            Mvx.RegisterSingleton<IUserDialogs>(UserDialogs.Instance);
            
            //db
            string externalDirPath = deviceProvider.DataPath();
            string dbPath = PortablePath.Combine(externalDirPath, FilesNames.databaseFile);
            var dbConnectionFactory = new DbConnectionFactory(dbPath);
            Mvx.RegisterSingleton<IDbConnectionFactory>(dbConnectionFactory);

            var storageProvider = new StorageProvider(Mvx.Resolve<IDeviceProvider>(), Mvx.Resolve<IDbConnectionFactory>());
            Mvx.RegisterSingleton<IStorageProvider>(storageProvider);

            var sessionProvider = new SessionProvider(Mvx.Resolve<IStorageProvider>());
            Mvx.RegisterSingleton<ISessionProvider>(sessionProvider);

            //api interfaces
            var carrierOrdersApi = RestService.For<ICarrierOrdersApi>(sessionProvider.HttpClient);
            Mvx.RegisterSingleton<ICarrierOrdersApi>(carrierOrdersApi);

            var routesApi = RestService.For<IRoutesApi>(sessionProvider.HttpClient);
            Mvx.RegisterSingleton<IRoutesApi>(routesApi);

            var salepointOrdersApi = RestService.For<ISalepointOrdersApi>(sessionProvider.HttpClient);
            Mvx.RegisterSingleton<ISalepointOrdersApi>(salepointOrdersApi);

            //services
            var carrierOrdersService = new CarrierOrdersService(Mvx.Resolve<ICarrierOrdersApi>(), Mvx.Resolve<INotificationsProvider>());
            Mvx.RegisterSingleton<ICarrierOrdersService>(carrierOrdersService);

            var salepointOrdersService = new SalepointOrdersService(Mvx.Resolve<ISalepointOrdersApi>(), Mvx.Resolve<IStorageProvider>(), Mvx.Resolve<INotificationsProvider>());
            Mvx.RegisterSingleton<ISalepointOrdersService>(salepointOrdersService);

            var routesService = new RoutesService(Mvx.Resolve<IRoutesApi>(), Mvx.Resolve<INotificationsProvider>(), Mvx.Resolve<ISessionProvider>());
            Mvx.RegisterSingleton<IRoutesService>(routesService);

            RegisterNavigationServiceAppStart<SignInViewModel>();
        }
    }
}
