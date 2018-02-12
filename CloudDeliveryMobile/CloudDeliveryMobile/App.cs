using CloudDeliveryMobile.ViewModels;
using MvvmCross.Core.ViewModels;
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
            RegisterNavigationServiceAppStart<SigninViewModel>();
        }
    }
}
