using CloudDeliveryMobile.Providers;
using CloudDeliveryMobile.Providers.Implementations;
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

            Mvx.RegisterType<IDeviceProvider, DeviceProvider>();
            Mvx.RegisterType<IHttpProvider, HttpProvider>();

            var storageProvider = new StorageProvider(Mvx.Resolve<IDeviceProvider>());
            Mvx.RegisterSingleton<IStorageProvider>(storageProvider);

            var sessionProvider = new SessionProvider(Mvx.Resolve<IHttpProvider>(), Mvx.Resolve<IStorageProvider>());
            Mvx.RegisterSingleton<ISessionProvider>(sessionProvider);            

            RegisterNavigationServiceAppStart<TokenSignInViewModel>();
        }
    }
}
