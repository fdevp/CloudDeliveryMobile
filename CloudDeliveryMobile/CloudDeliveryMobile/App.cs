using Acr.UserDialogs;
using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Providers.Implementations;
using CloudDeliveryMobile.Resources;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.Services.Implementations;
using CloudDeliveryMobile.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using PCLStorage;

namespace CloudDeliveryMobile
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {

            var deviceProvider = new DeviceProvider();
            Mvx.RegisterSingleton<IDeviceProvider>(deviceProvider);

            var httpProvider = new HttpProvider();
            Mvx.RegisterSingleton<IHttpProvider>(httpProvider);

            Mvx.RegisterSingleton<IUserDialogs>(UserDialogs.Instance);

            //db
            string externalDirPath = deviceProvider.DataPath();
            string dbPath = PortablePath.Combine(externalDirPath, FilesNames.databaseFile);
            var dbConnectionFactory = new DbConnectionFactory(dbPath);
            Mvx.RegisterSingleton<IDbConnectionFactory>(dbConnectionFactory);

            var storageProvider = new StorageProvider(Mvx.Resolve<IDeviceProvider>(), Mvx.Resolve<IDbConnectionFactory>());
            Mvx.RegisterSingleton<IStorageProvider>(storageProvider);

            var sessionProvider = new SessionProvider(Mvx.Resolve<IHttpProvider>(), Mvx.Resolve<IStorageProvider>());
            Mvx.RegisterSingleton<ISessionProvider>(sessionProvider);

            var carrierOrdersService = new CarrierOrdersService(Mvx.Resolve<IHttpProvider>());
            Mvx.RegisterSingleton<ICarrierOrdersService>(carrierOrdersService);

            var salepointOrdersService = new SalepointOrdersService(Mvx.Resolve<IHttpProvider>(), Mvx.Resolve<IStorageProvider>());
            Mvx.RegisterSingleton<ISalepointOrdersService>(salepointOrdersService);

            var routesService = new RoutesService(Mvx.Resolve<IHttpProvider>(), Mvx.Resolve<IStorageProvider>());
            Mvx.RegisterSingleton<IRoutesService>(routesService);

            RegisterNavigationServiceAppStart<SignInViewModel>();
        }
    }
}
