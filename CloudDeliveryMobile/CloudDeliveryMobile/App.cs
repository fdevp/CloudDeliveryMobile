using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Providers.Implementations;
using CloudDeliveryMobile.Services;
using CloudDeliveryMobile.Services.Implementations;
using CloudDeliveryMobile.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            var storageProvider = new StorageProvider(Mvx.Resolve<IDeviceProvider>());
            Mvx.RegisterSingleton<IStorageProvider>(storageProvider);

            var sessionProvider = new SessionProvider(Mvx.Resolve<IHttpProvider>(), Mvx.Resolve<IStorageProvider>());
            Mvx.RegisterSingleton<ISessionProvider>(sessionProvider);

            var ordersService = new OrdersService(Mvx.Resolve<IHttpProvider>());
            Mvx.RegisterSingleton<IOrdersService>(ordersService);


            RegisterNavigationServiceAppStart<TokenSignInViewModel>();
        }
    }
}
